using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FluentAssertions;
using LiveAuction.Application.Dtos.AuthModel;
using LiveAuction.Application.Helpers;
using LiveAuction.Application.Interfaces;
using LiveAuction.Application.Profiles;
using LiveAuction.Application.Services;
using LiveAuction.Core.Entites;
using LiveAuction.Core.Entites.AuthEntites;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;

namespace LiveAuction.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IOptions<JWT>> _mockJwtOptions;
        private readonly IMapper _realMapper;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IGenericRepository<RefreshToken>> _mockRefreshTokenRepo;

        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserManager = MockUserManager();
            _mockJwtOptions = new Mock<IOptions<JWT>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockRefreshTokenRepo = new Mock<IGenericRepository<RefreshToken>>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<AuthProfile>());
            _realMapper = config.CreateMapper();


            _mockJwtOptions.Setup(x => x.Value).Returns(new JWT
            {
                Key = "SecretKey123456789SecretKey123456789",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                AccessTokenValidityInMinutes = 60,
            });

            _authService = new AuthService(
                _mockUserManager.Object,
                _mockJwtOptions.Object,
                _realMapper,
                _mockConfiguration.Object,
                _mockRefreshTokenRepo.Object
            );
        }
        private static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }
        #region RegisterAsync Tests
        [Fact]
        public async Task RegisterAsync_ShouldReturnFailure_WhenEmailAlreadyExists()
        {
            //Arrange
            var registerModel = new RegisterModel
            {
                Email = "exsiting@gmail.com",
                Password = "Password123!",
                FullName = "Test User",
                UserName = "TestUser"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(registerModel.Email)).
                ReturnsAsync(new ApplicationUser { Email = registerModel.Email });


            //Act
            var result = await _authService.RegisterAsync(registerModel);


            //Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Email is already registered");
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnSuccess_WhenRegistrationIsValid()
        {
            //Arrange
            var registerModel = new RegisterModel
            {
                Email = "Test@gmail.com",
                Password = "Password123!",
                FullName = "Test User",
                UserName = "TestUser"
            };
            _mockUserManager.Setup(x => x.FindByEmailAsync(registerModel.Email))
                .ReturnsAsync((ApplicationUser?)null);

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).
               ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User")).
               ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).
                ReturnsAsync(new List<string> { "User", "Admin" });
            _mockUserManager.Setup(x => x.GetClaimsAsync(It.IsAny<ApplicationUser>())).
                ReturnsAsync(new List<Claim>());


            _mockRefreshTokenRepo.Setup(x => x.AddAsync(It.IsAny<RefreshToken>()))
                 .ReturnsAsync((RefreshToken r) => r);


            //Act
            var result = await _authService.RegisterAsync(registerModel);


            //Assert
            result.Succeeded.Should().BeTrue();
            result.Message.Should().Be("User registered successfully");
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnFailure_WhenUserCreationFails()
        {
            //Arrange
            var registerModel = new RegisterModel
            {
                Email = "Test@gmail.com",
                Password = "Password123!",
                FullName = "Test User",
                UserName = "TestUser"
            };
            _mockUserManager.Setup(x => x.FindByEmailAsync(registerModel.Email)).
                ReturnsAsync((ApplicationUser?)null);


            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerModel.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed" }));


            //Act
            var result = await _authService.RegisterAsync(registerModel);


            //Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Error occured while created account");
            result.Data.Should().BeNull();
        }



        #endregion

        #region LoginAsync Tests
        [Fact]
        public async Task GetTokenAsync_ShouldReturnFailure_WhenEmailDoesNotExist()
        {
            //Arrange
            var tokenRequestModel = new TokenRequestModel
            {
                Email = "Test@gmail.com",
                Password = "Password123!"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(tokenRequestModel.Email)).
                ReturnsAsync((ApplicationUser?)null);

            //Act
            var result = await _authService.GetTokenAsync(tokenRequestModel);

            //Assert

            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Email or Password is incorrect");
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task GetTokenAsync_ShouldSucceed_WhenCredentialsAreValid()
        {
            //Arrange
            var tokenRequestModel = new TokenRequestModel
            {
                Email = "Test@gmail.com",
                Password = "Password123!"
            };

            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).
                ReturnsAsync(new List<string> { "Admin", "User" });

            _mockUserManager.Setup(x => x.GetClaimsAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<Claim>());

            _mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), tokenRequestModel.Password))
                .ReturnsAsync(true);

            _mockUserManager.Setup(x => x.FindByEmailAsync(tokenRequestModel.Email))
                .ReturnsAsync(new ApplicationUser { Email = tokenRequestModel.Email, UserName = "TestUser" });

            _mockRefreshTokenRepo.Setup(x => x.ListAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                .ReturnsAsync(new List<RefreshToken>());


            //Act
            var result = await _authService.GetTokenAsync(tokenRequestModel);

            //Assert
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().BeOfType<AuthModel>();
        }
        #endregion

        #region RefreshTokenAsync Tests
        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnFailure_WhenTokenIsInvalid()
        {
            //Arrange
            var refreshToken = "invalid refresh token";

            _mockRefreshTokenRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                .ReturnsAsync((RefreshToken?)null);
            //Act
            var result = await _authService.RefreshTokenAsync(refreshToken);

            //Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Invalid refresh token");
            result.Data.Should().BeNull();
        }


        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            //Arrange
            var refreshToken = "invalid refresh token";

            _mockRefreshTokenRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                .ReturnsAsync(new RefreshToken());

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            //Act
            var result = await _authService.RefreshTokenAsync(refreshToken);

            //Assert

            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("User not found");
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnFailure_WhenRefreshTokenIsRevoked()
        {
            //Arrange
            var refreshToken = "invalid refresh token";

            _mockRefreshTokenRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                .ReturnsAsync(new RefreshToken()
                {
                    Revoked = DateTime.UtcNow.AddDays(-1)
                });


            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser());

            //Act
            var result = await _authService.RefreshTokenAsync(refreshToken);

            //Assert

            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Security breach detected!");
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnFailure_WhenRefreshTokenIsExpired()
        {
            //Arrange
            var refreshToken = "invalid refresh token";

            _mockRefreshTokenRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                .ReturnsAsync(new RefreshToken()
                {
                    Revoked = null,
                    Expires = DateTime.UtcNow.AddDays(-1)
                });


            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser());

            //Act
            var result = await _authService.RefreshTokenAsync(refreshToken);

            //Assert

            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Refresh token expired");
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnSuccessed_WhenRefreshTokenIsValid()
        {
            //Arrange
            var refreshToken = "valid refresh token";

            _mockRefreshTokenRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                .ReturnsAsync(new RefreshToken()
                {
                    Revoked = null,
                    Expires = DateTime.UtcNow.AddDays(1)
                });

            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).
                ReturnsAsync(new List<string> { "Admin", "User" });

            _mockUserManager.Setup(x => x.GetClaimsAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<Claim>());


            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser()
                {
                    UserName = "test user",
                    Email = "E@test.com",
                    FullName = "test",
                    ConcurrencyStamp = Guid.NewGuid().ToString()

                });


            _mockRefreshTokenRepo.Setup(x => x.ListAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>())).
                ReturnsAsync(new List<RefreshToken>());



            //Act
            var result = await _authService.RefreshTokenAsync(refreshToken);

            //Assert

            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().BeOfType<AuthModel>();

        }
        #endregion

        #region RevokeTokenAsync Tests

        [Fact]
        public async Task RevokeTokenAsync_ShouldReturnFailure_WhenRefrehTokenNotFound()
        {
            //Arrange
            var refreshToken = "refresh toekn";
            _mockRefreshTokenRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>())).
                ReturnsAsync((RefreshToken?)null);

            //Act
            var result = await _authService.RevokeTokenAsync(refreshToken);


            //Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Refresh token not found");

        }
        [Fact]
        public async Task RevokeTokenAsync_ShouldBeFailure_WhenRefreshTokenIsNotActive()
        {
            //Arrange
            var refreshToken = "refresh token";
            _mockRefreshTokenRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>())).
                ReturnsAsync(new RefreshToken()
                {
                    Revoked = DateTime.UtcNow.AddDays(-1),
                    Expires = DateTime.UtcNow.AddDays(-1)
                });


            //Act
            var result = await _authService.RevokeTokenAsync(refreshToken);


            //Assert
            result.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task RevokeTokenAsync_ShouldBeFailure_WhenRefreshTokenIsActive()
        {
            //Arrange
            var refreshToken = "refresh toekn";
            _mockRefreshTokenRepo.Setup(x => x.GetAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>())).
                ReturnsAsync(new RefreshToken()
                {
                    Revoked = null,
                    Expires = DateTime.UtcNow.AddDays(1)
                });
            _mockRefreshTokenRepo.Setup(x => x.UpdateAsync(It.IsAny<RefreshToken>())).
                Returns(Task.CompletedTask);

            //Act
            var result = await _authService.RevokeTokenAsync(refreshToken);


            //Assert
            result.Succeeded.Should().BeTrue();
        }

        #endregion


    }
}