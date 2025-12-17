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
        

        
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<GitHubProjectV2ResponseDto> GetProjectV2Async(
            string owner,
            int projectNumber,
            int itemsFirst = 50)
        {
            var response = await _httpClient.GetAsync(
                $"api/github/projectv2?owner={owner}&number={projectNumber}&itemsFirst={itemsFirst}");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<GitHubProjectV2ResponseDto>(json, _jsonOptions);

            return result ?? throw new JsonException("Deserialization returned null.");
        }


    }
}
