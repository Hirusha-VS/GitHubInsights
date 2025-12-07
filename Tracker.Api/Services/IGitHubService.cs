using Tracker.Api.Dtos;

namespace Tracker.Api.Services
{
    public interface IGitHubService
    {
        Task<UserDto> GetUserAsync();
        Task<List<RepositoryDto>> GetUserRepositoriesAsync();
        Task<RepositoryStatsDto> GetRepositoryStatsAsync(string owner, string repositoryName);
        Task<List<CommitDto>> GetCommitsAsync(string owner, string repoName);
        Task<List<PullRequestDto>> GetPullRequestsAsync(string owner, string repoName);
        Task<List<IssueDto>> GetIssuesAsync(string owner, string repoName);

        Task<List<string>> GetUserOrganizationsAsync();
        Task<GitHubProjectV2ResponseDto?> GetProjectV2Async(string owner, int projectNumber,int itemsFirst);
    }
}
