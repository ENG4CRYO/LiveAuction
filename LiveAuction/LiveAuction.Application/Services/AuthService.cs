using AutoMapper;
using LiveAuction.Application.Common;
using LiveAuction.Application.Dtos.AuthModel;
using LiveAuction.Application.Helpers;
using LiveAuction.Application.Interfaces;
using LiveAuction.Application.Models;
using LiveAuction.Core.Entites;
using LiveAuction.Core.Entites.AuthEntites;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace LiveAuction.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly TokenHelper _tokenHelper;
        private readonly IConfiguration _configuration;
        private readonly JWT _jwt;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<RefreshToken> _refreshTokenRepo;
        private readonly IMemoryCache _memoryCache;
        private readonly IEmailQueue _emailQueue;

        public AuthService(UserManager<ApplicationUser> userManager,
        IOptions<JWT> jwt,
        IMapper mapper,
        IConfiguration configuration,
        IGenericRepository<RefreshToken> refreshTokenRepo,
        IMemoryCache memoryCache,
        IEmailService emailService,
        IEmailQueue emailQueue
        )
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _mapper = mapper;
            _configuration = configuration;
            _tokenHelper = new TokenHelper(_jwt, _userManager);
            _refreshTokenRepo = refreshTokenRepo;
            _memoryCache = memoryCache;
            _emailQueue = emailQueue;

        }

        public async Task<ApiResponse<AuthModel>> GetTokenAsync(TokenRequestModel tokenRequestModel)
        {
            var ApiResponse = new ApiResponse<AuthModel>();
            var authModel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(tokenRequestModel.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, tokenRequestModel.Password))
            {
                authModel.IsAuthenticated = false;
                ApiResponse.Data = authModel;
                return ApiResponse<AuthModel>.Failure("Email or Password is incorrect");
            }

            var jwtSecurityToken = await _tokenHelper.CreateJwtToken(user);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);


            await _tokenHelper.ManageUserTokensAsync(_refreshTokenRepo, user.Id);
            var refreshToken = _tokenHelper.GenerateRefreshToken();
            refreshToken.UserId = user.Id;

            await _refreshTokenRepo.AddAsync(refreshToken);


            var roles = await _userManager.GetRolesAsync(user);
            authModel = _mapper.Map<AuthModel>(user);

            authModel.IsAuthenticated = true;
            authModel.Token = tokenString;
            authModel.RefreshToken = refreshToken.Token;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.RefreshTokenExpiration = refreshToken.Expires;
            authModel.Roles = roles;


            return ApiResponse<AuthModel>.Success(authModel);

        }

        public async Task<ApiResponse<AuthModel>> RegisterAsync(RegisterModel registerModel)
        {
            var ApiResponse = new ApiResponse<AuthModel>();
            var authModel = new AuthModel();
            var verifiedEmail = await _tokenHelper.ExtractEmailFromRegisterToken(registerModel.RegisterToken);

            if (string.IsNullOrEmpty(verifiedEmail))
            {
                return ApiResponse<AuthModel>.Failure("Invalid or expired Register Token. Please verify your email again");
            }

            var userExists = await _userManager.FindByEmailAsync(verifiedEmail);
            if (userExists != null)
            {
                return ApiResponse<AuthModel>.Failure("Email is already registered");
            }

            var newUser = _mapper.Map<ApplicationUser>(registerModel);
            newUser.ConcurrencyStamp = Guid.NewGuid().ToString();
            newUser.Email = verifiedEmail;
            newUser.UserName = verifiedEmail;
            var result = await _userManager.CreateAsync(newUser, registerModel.Password);

            if (!result.Succeeded)
            {
                authModel.IsAuthenticated = false;
                var errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }
                ApiResponse.Data = authModel;

                return ApiResponse<AuthModel>.Failure("Error occured while created account");
            }

            await _userManager.AddToRoleAsync(newUser, "User");

            var newRefreshToken = _tokenHelper.GenerateRefreshToken();
            newRefreshToken.UserId = newUser.Id;
            await _refreshTokenRepo.AddAsync(newRefreshToken);

            var token = await _tokenHelper.CreateJwtToken(newUser);


            authModel = _mapper.Map<AuthModel>(newUser);
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(token);
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.ExpiresOn = token.ValidTo;
            authModel.RefreshTokenExpiration = newRefreshToken.Expires;
            authModel.IsAuthenticated = true;
            authModel.Roles = new List<string>() { "User" };

            return ApiResponse<AuthModel>.Success(authModel, "User registered successfully");

        }

        public async Task<ApiResponse<AuthModel>> RefreshTokenAsync(string refreshToken)
        {

            var storedToken = await _refreshTokenRepo.GetAsync(t => t.Token == refreshToken);


            if (storedToken == null)
                return ApiResponse<AuthModel>.Failure("Invalid refresh token");

            var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
            if (user == null) return ApiResponse<AuthModel>.Failure("User not found");

            if (storedToken.IsRevoked)
            {
                return ApiResponse<AuthModel>.Failure("Security breach detected!");
            }

            if (!storedToken.IsActive)
                return ApiResponse<AuthModel>.Failure("Refresh token expired");

            await _tokenHelper.ManageUserTokensAsync(_refreshTokenRepo, user.Id);
            var newRefreshToken = _tokenHelper.GenerateRefreshToken();
            newRefreshToken.UserId = user.Id;


            storedToken.Revoked = DateTime.UtcNow;
            storedToken.ReasonRevoked = "Replaced by new token";
            storedToken.ReplacedByToken = newRefreshToken.Token;


            await _refreshTokenRepo.UpdateAsync(storedToken);
            await _refreshTokenRepo.AddAsync(newRefreshToken);

            var jwtSecurityToken = await _tokenHelper.CreateJwtToken(user);

            var authModel = _mapper.Map<AuthModel>(user);
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = await _userManager.GetRolesAsync(user);
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.Expires;

            return ApiResponse<AuthModel>.Success(authModel);
        }

        public async Task<ApiResponse<bool>> RevokeTokenAsync(string token)
        {

            var storedToken = await _refreshTokenRepo.GetAsync(t => t.Token == token);


            if (storedToken == null)
                return ApiResponse<bool>.Failure("Refresh token not found");

            if (storedToken.IsActive)
            {
                storedToken.Revoked = DateTime.UtcNow;
                storedToken.ReasonRevoked = "Revoked manually by user (Logout)";

                await _refreshTokenRepo.UpdateAsync(storedToken);
            }

            return ApiResponse<bool>.Success(true);
        }


        public async Task<ApiResponse<string>> RequestOtpAsync(OtpRequestModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                return ApiResponse<string>.Failure("Email is already registered");
            }

            var otp = new Random().Next(1000, 9999).ToString();

            var cacheKey = $"OTP_{model.Email}";
            _memoryCache.Set(cacheKey, otp, TimeSpan.FromMinutes(5));

            string emailBody = $@"
        <h3>Welcome to LiveAuction!</h3>
        <p>Your verification code is: <b style='font-size: 20px; color: blue;'>{otp}</b></p>
        <p>This code is valid for 5 minutes.</p>";

            await _emailQueue.QueueBackgroundEmailAsync(
                 new EmailMetadata(model.Email, "LiveAuction Verification Code", emailBody));
            return ApiResponse<string>.Success($"Code verfication sent to {model.Email}");
        }

        public async Task<ApiResponse<string>> VerifyOtpAsync(OtpVerifyModel model)
        {
            var cacheKey = $"OTP_{model.Email}";
            if (!_memoryCache.TryGetValue(cacheKey, out string? storedOtp))
            {
                return ApiResponse<string>.Failure("OTP has expired or does not exist");
            }

            if (storedOtp != model.Otp)
            {
                return ApiResponse<string>.Failure("Invalid OTP code");
            }

            _memoryCache.Remove(cacheKey);

            var registerToken = await _tokenHelper.GenerateRegisterToken(model.Email);

            return ApiResponse<string>.Success(registerToken, "OTP Verified. Use this token to complete registration");
        }

    }
}
