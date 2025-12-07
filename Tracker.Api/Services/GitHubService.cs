using Octokit;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Tracker.Api.Dtos;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tracker.Api.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _client;

        //to access graphql api
        private readonly HttpClient _httpClient;

        public GitHubService(HttpClient http,IConfiguration config)
        {
            _httpClient = http;
            var token = config["GitHub:Token"];

            _client = new GitHubClient(new Octokit.ProductHeaderValue("GitHubDashboard"))
            {
                Credentials = new Credentials(token)
            };

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TrackerApp");
        }

        //GraphQL METHODS

        public async Task<GitHubProjectV2ResponseDto?> GetProjectV2Async(
            string owner,
            int projectNumber,
            int itemsFirst = 100)
                {
                    var query = @"
                query GetProjectV2($owner: String!, $projectNumber: Int!, $itemsFirst: Int!) {
                  organization(login: $owner) {
                    projectV2(number: $projectNumber) {
                      id
                      title
                      url
                      views(first: 20) {
                        nodes {
                          id
                          name
                          number
                          layout
                        }
                      }
                      items(first: $itemsFirst) {
                        nodes {
                          id
                          type
                          content {
                            ... on Issue { 
                              title 
                              url 
                              number 
                              state 
                            }
                            ... on PullRequest { 
                              title 
                              url 
                              number 
                              merged 
                              state 
                            }
                            ... on DraftIssue { 
                              title 
                            }
                          }
                          fieldValues(first: 20) {
                            nodes {
                              __typename
                              ... on ProjectV2ItemFieldTextValue {
                                text
                                field { 
                                  ... on ProjectV2FieldCommon {
                                    name
                                  }
                                }
                              }
                              ... on ProjectV2ItemFieldSingleSelectValue {
                                name
                                field { 
                                  ... on ProjectV2FieldCommon {
                                    name
                                  }
                                }
                              }
                              ... on ProjectV2ItemFieldNumberValue {
                                number
                                field { 
                                  ... on ProjectV2FieldCommon {
                                    name
                                  }
                                }
                              }
                              ... on ProjectV2ItemFieldDateValue {
                                date
                                field { 
                                  ... on ProjectV2FieldCommon {
                                    name
                                  }
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }";

                    var body = new
                    {
                        query,
                        variables = new
                        {
                            owner,              // FIX: Use the parameter, not hardcoded value
                            projectNumber,      // FIX: Use the parameter, not hardcoded value
                            itemsFirst
                        }
                    };

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true  // FIX: Handle camelCase from GraphQL
                    };

                    var json = JsonSerializer.Serialize(body, options);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync("https://api.github.com/graphql", content);
                    var jsonResp = await response.Content.ReadAsStringAsync();

                    Console.WriteLine($"GraphQL Response: {jsonResp}");

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"GitHub GraphQL API HTTP error: {response.StatusCode} - {jsonResp}");

                    // FIX: Check for GraphQL errors in the response
                    var errorCheck = JsonSerializer.Deserialize<GraphQLErrorResponse>(jsonResp, options);
                    if (errorCheck?.errors != null && errorCheck.errors.Any())
                    {
                        var errorMessages = string.Join("; ", errorCheck.errors.Select(e => e.message));
                        throw new Exception($"GitHub GraphQL API error: {errorMessages}");
                    }

                    return JsonSerializer.Deserialize<GitHubProjectV2ResponseDto>(jsonResp, options);
                }

                // Add this helper class for error checking
                private class GraphQLErrorResponse
                {
                    public List<GraphQLError>? errors { get; set; }
                }

                private class GraphQLError
                {
                    public string message { get; set; }
                    public string type { get; set; }
                }










        //REST API METHODS
        public async Task<List<CommitDto>> GetCommitsAsync(string owner, string repoName)
        {
            var commits = await _client.Repository.Commit.GetAll(owner, repoName);
            return commits.Select(c => new CommitDto
            {
                Sha = c.Sha,
                Message = c.Commit.Message,
                AuthorName = c.Commit.Author.Name,
                Date = c.Commit.Author.Date
            }).ToList();
        }

        public async Task<List<IssueDto>> GetIssuesAsync(string owner, string repoName)
        {
            var  issues = await _client.Issue.GetAllForRepository(owner, repoName);

            return issues.Select(i => new IssueDto
            {
                Number = i.Number,
                Title = i.Title,
                Author = i.User.Login,
                CreatedAt = i.CreatedAt,
                Url = i.HtmlUrl,
                State = i.State.StringValue,
                IsPullRequest = i.PullRequest != null
            }).ToList();
        }

        public async Task<List<PullRequestDto>> GetPullRequestsAsync(string owner, string repoName)
        {
            var pullRequests = await _client.PullRequest.GetAllForRepository(owner, repoName);
            return pullRequests.Select(pr => new PullRequestDto
            {
                Number = pr.Number,
                Title = pr.Title,
                Author = pr.User.Login,
                CreatedAt = pr.CreatedAt,
                Url = pr.HtmlUrl,
                State = pr.State.StringValue
            }).ToList();
        }

        public async Task<RepositoryStatsDto> GetRepositoryStatsAsync(string owner, string repositoryName)
        {
            var repoStats = await _client.Repository.Get(owner, repositoryName);
            return new RepositoryStatsDto
            {
                Name = repoStats.Name,
                Stars = repoStats.StargazersCount,
                Forks = repoStats.ForksCount,
                OpenIssues = repoStats.OpenIssuesCount,
                Language = repoStats.Language ?? "N/A",
                Owner = new OwnerDto
                {
                    Login = repoStats.Owner.Login
                }
            };
        }

        public async Task<UserDto> GetUserAsync()
        {
            var user = await _client.User.Current();
            return new UserDto
            {
                Login = user.Login,
                Name = user.Name,
                Avatar_url = user.AvatarUrl,
                Bio = user.Bio,
                Followers = user.Followers,
                Following = user.Following,
                PublicRepos = user.PublicRepos
            };
        }

        public async Task<List<string>> GetUserOrganizationsAsync()
        {
            var orgs = await _client.Organization.GetAllForCurrent();

            return orgs.Select(o => o.Login).ToList();
        }

        public async Task<List<RepositoryDto>> GetUserRepositoriesAsync()
        {
            var repos = await _client.Repository.GetAllForCurrent();
            return repos.Select(r => new RepositoryDto
            {
                Name = r.Name,
                Description = r.Description,
                Language = r.Language ?? "N/A",
                Stars = r.StargazersCount,
                Forks = r.ForksCount,
                UpdatedAt = r.UpdatedAt,
                OpenIssues = r.OpenIssuesCount,
                Owner = new OwnerDto
                {
                    Login = r.Owner.Login
                }
            }).ToList();

        }
    }
}
