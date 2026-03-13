using Asp.Versioning;
using LiveAuction.API.Extensions;
using LiveAuction.Application.Interfaces.UserProfileInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace LiveAuction.API.Controllers.V1
{
    //[Authorize]
    //[ApiController]
    //[Route("liveauction/[Controller]")]
    //[ApiVersion("1.0")]
    //[EnableRateLimiting("IpLimiter")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet]
        [EndpointDescription("Retrieves the profile information of a user, including their name, profile picture, bio, and auction statistics.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserProfile(CancellationToken cancellationToken)
        {
            var userId = ClaimsPrincipalExtensions.GetUserId(User);
            if (userId == Guid.Empty)
            {
                return BadRequest("Invalid user ID.");
            }
            var result = await _userProfileService.GetUserProfileAsync(userId, cancellationToken);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
