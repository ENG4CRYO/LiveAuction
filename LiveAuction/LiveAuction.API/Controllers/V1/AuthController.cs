using Asp.Versioning;
using LiveAuction.Application.Common;
using LiveAuction.Application.Dtos.AuthModel;
using LiveAuction.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LiveAuction.API.Controllers.V1
{
    [ApiController]
    [Route("api/[Controller]")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [EndpointDescription("It creates a new user account and returns the token and user details. It requires a unique email address.")]
        [ProducesResponseType(typeof(ApiResponse<AuthModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _authService.RegisterAsync(model);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        [EndpointDescription("Verifying user cridential,Issue Token and refresh token for user")]
        [ProducesResponseType(typeof(ApiResponse<AuthModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] TokenRequestModel model)
        {
            var result = await _authService.GetTokenAsync(model);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }


        [HttpPost("refresh-token")]
        [EndpointDescription("Verify refresh token, rovoke the refresh token, issue new refresh token")]
        [ProducesResponseType(typeof(ApiResponse<AuthModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody] RequestRefreshToken refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken.Token);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }


        [HttpPost("logout")]
        [EndpointDescription("revoke the refresh token")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest model)
        {


            if (string.IsNullOrEmpty(model.Token))
                return BadRequest(ApiResponse<string>.Failure("Token is required!"));

            var result = await _authService.RevokeTokenAsync(model.Token);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(ApiResponse<string>.Success("Token revoked successfully"));
        }


    }
}