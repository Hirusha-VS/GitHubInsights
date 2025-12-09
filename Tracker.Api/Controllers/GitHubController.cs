using Microsoft.AspNetCore.Mvc;
using Tracker.Api.Services;

namespace Tracker.Api.Controllers
{
    [ApiController]
    [Route("api/github")]
    public class GitHubController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;
        

        public GitHubController(IGitHubService? gitHubService)
        {
            _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
        }

        [HttpGet("organizations")]
        public async Task<IActionResult> GetOrgs()
        {
            var orgs = await _gitHubService.GetUserOrganizationsAsync() ;
            return Ok(orgs);
        }


        [HttpGet("projectv2")]
        public async Task<IActionResult> GetProjectV2([FromQuery] string owner, [FromQuery] int number, int itemsFirst = 50)
        {
            var data = await _gitHubService.GetProjectV2Async(owner, number, itemsFirst);
            return Ok(data);
        }




    }
}
