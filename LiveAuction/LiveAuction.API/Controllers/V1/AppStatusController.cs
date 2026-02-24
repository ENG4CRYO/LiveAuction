using Asp.Versioning;
using LiveAuction.Application.Common;
using LiveAuction.Application.Dtos.AppStatusModel;
using LiveAuction.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.VisualBasic;

namespace LiveAuction.API.Controllers.V1
{
    [ApiController]
    [Route("liveauction/[Controller]")]
    [ApiVersion("1.0")]
    [EnableRateLimiting("IpLimiter")]
    public class AppStatusController : ControllerBase
    {
        private readonly IAppStatusService _appStatusService;

        public AppStatusController(IAppStatusService appStatusService)
        {
            _appStatusService = appStatusService;
        }

        [HttpGet("check")]
        [EndpointDescription("Checks the current status of the application, including maintenance mode, required updates, and ban status.")]
        [ProducesResponseType(typeof(ApiResponse<AppStatusResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckStatus([FromQuery] string clientVersion, [FromQuery] string os)
        {
            var result = await _appStatusService.CheckStatusAsync(clientVersion, os);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);

        }

    }
}
