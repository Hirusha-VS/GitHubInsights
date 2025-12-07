using Microsoft.AspNetCore.Mvc;
using Tracker.Api.Services;

namespace Tracker.Api.Controllers
{
    [ApiController]
    [Route("api/github")]
    public class GitHubController : ControllerBase
    {
        private readonly IGitHubService? _gitHubService;
        

        public GitHubController(IGitHubService? gitHubService)
        {
            _gitHubService = gitHubService;
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            var user = await _gitHubService!.GetUserAsync();
            return Ok(user);
        }

        [HttpGet("repositories")]
        public async Task<IActionResult> GetRepositories()
        {
            var repos = await _gitHubService!.GetUserRepositoriesAsync();
            return Ok(repos);
        }

        [HttpGet("repositories/{owner}/{repo}/stats")]
        public async Task<IActionResult> GetRepoStats(string owner, string repo)
        {
            var stats = await _gitHubService!.GetRepositoryStatsAsync(owner, repo);
            return Ok(stats);
        }

        [HttpGet("repositories/{owner}/{repo}/commits")]
        public async Task<IActionResult> GetCommits(string owner, string repo)
        {
            var commits = await _gitHubService!.GetCommitsAsync(owner, repo);
            return Ok(commits);
        }

        [HttpGet("repositories/{owner}/{repo}/pulls")]
        public async Task<IActionResult> GetPullRequests(string owner, string repo)
        {
            var pulls = await _gitHubService!.GetPullRequestsAsync(owner, repo);
            return Ok(pulls);
        }

        [HttpGet("repositories/{owner}/{repo}/issues")]
        public async Task<IActionResult> GetIssues(string owner, string repo)
        {
            var issues = await _gitHubService!.GetIssuesAsync(owner, repo);
            return Ok(issues);
        }

        [HttpGet("organizations")]
        public async Task<IActionResult> GetOrgs()
        {
            var orgs = await _gitHubService!.GetUserOrganizationsAsync();
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
