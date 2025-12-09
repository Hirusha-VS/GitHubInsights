using Tracker.Api.Dtos;

namespace Tracker.Api.Services
{
    public interface IGitHubService
    {
     
        Task<List<string>> GetUserOrganizationsAsync();
        Task<GitHubProjectV2ResponseDto?> GetProjectV2Async(string owner, int projectNumber,int itemsFirst);
    }
}
