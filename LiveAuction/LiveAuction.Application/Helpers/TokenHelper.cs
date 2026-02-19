using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LiveAuction.Application.Interfaces;
using LiveAuction.Core.Entites;
using LiveAuction.Core.Entites.AuthEntites;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace LiveAuction.Application.Helpers
{
    public class TokenHelper
    {
        private readonly JWT _jwt;
        private readonly UserManager<ApplicationUser> _userManager;
        public TokenHelper(JWT jwt, UserManager<ApplicationUser> userManager)
        {
            _jwt = jwt;
            _userManager = userManager;
        }
        public async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user, IList<string> roles)
        {

            var userClaims = await _userManager.GetClaimsAsync(user);

            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            }.Union(userClaims).Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.AccessTokenValidityInMinutes),
                signingCredentials: signingCredentials
                );

            return jwtSecurityToken;
        }

        public RefreshToken GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(32);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(_jwt.RefreshTokenValidityInDays),
                Created = DateTime.UtcNow
            };


        }

        public async Task ManageUserTokensAsync(IGenericRepository<RefreshToken> _refreshTokenRepo, Guid userId)
        {
            var expiredTokens = await _refreshTokenRepo.ListAsync(t => t.UserId == userId && t.Expires <= DateTime.UtcNow)
                ?? Enumerable.Empty<RefreshToken>(); ;
            if (expiredTokens.Any())
            {
                await _refreshTokenRepo.DeleteRangeAsync(expiredTokens);
            }

            const int MaxActiveSessions = 5;

            var activeTokens = await _refreshTokenRepo.ListAsync(t =>
                t.UserId == userId && t.Revoked == null && t.Expires > DateTime.UtcNow);

            if (activeTokens.Count >= MaxActiveSessions)
            {

                var tokensToRevokeCount = activeTokens.Count - MaxActiveSessions + 1;

                var tokensToRevoke = activeTokens
                    .OrderBy(t => t.Created)
                    .Take(tokensToRevokeCount)
                    .ToList();

                foreach (var token in tokensToRevoke)
                {
                    token.Revoked = DateTime.UtcNow;
                    token.ReasonRevoked = "Exceeded max active sessions";
                }
                await _refreshTokenRepo.UpdateRangeAsync(tokensToRevoke);
            }
        }

        public async Task<string> GenerateRegisterToken(string email)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("purpose", "registration_only")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string?> ExtractEmailFromRegisterToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();    
            var key = Encoding.UTF8.GetBytes(_jwt.Key);

            try
            {
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwt.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var purpose = jwtToken.Claims.FirstOrDefault(x => x.Type == "purpose")?.Value;
                if (purpose != "registration_only") return null;

                return jwtToken.Claims.FirstOrDefault(
                    x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
            }
            catch
            {
                return null;
            }
        }

    }
}
