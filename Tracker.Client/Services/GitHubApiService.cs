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
