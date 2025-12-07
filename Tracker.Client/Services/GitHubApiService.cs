using System.Net.Http.Json;
using System.Text.Json;
using Tracker.Client.Dtos;
using static System.Net.WebRequestMethods;

namespace Tracker.Client.Services
{
    public class GitHubApiService
    {
        private readonly HttpClient _httpClient;

        public GitHubApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<UserDto> GetUserAsync() => 
            await _httpClient.GetFromJsonAsync<UserDto>("api/github/user");

        public async Task<List<RepositoryDto>> GetRepositoriesAsync() => 
            await _httpClient.GetFromJsonAsync<List<RepositoryDto>>("api/github/repositories");

        public async Task<RepositoryStatsDto> GetRepoStatsAsync(string owner, string repo) =>
            await _httpClient.GetFromJsonAsync<RepositoryStatsDto>($"api/github/repositories/{owner}/{repo}/stats");

        public async  Task<List<CommitDto>> GetCommitsAsync(string owner, string repo) =>
            await _httpClient.GetFromJsonAsync<List<CommitDto>>($"api/github/repositories/{owner}/{repo}/commits");

        public async Task<List<PullRequestDto>> GetPullRequestsAsync(string owner, string repo) =>
            await _httpClient.GetFromJsonAsync<List<PullRequestDto>>($"api/github/repositories/{owner}/{repo}/pulls");

        public async Task<List<IssueDto>> GetIssuesAsync(string owner, string repo) =>
            await _httpClient.GetFromJsonAsync<List<IssueDto>>($"api/github/repositories/{owner}/{repo}/issues");

        public async Task<List<string>> GetOrganizationsAsync() =>
            await _httpClient.GetFromJsonAsync<List<string>>("api/github/organizations");

        public async Task<GitHubProjectV2ResponseDto> GetProjectV2Async(string owner, int projectNumber, int itemsFirst = 50)
        {
            var response = await _httpClient.GetAsync($"api/github/projectv2?owner={owner}&number={projectNumber}&itemsFirst={itemsFirst}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GitHubProjectV2ResponseDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }




    }
}
