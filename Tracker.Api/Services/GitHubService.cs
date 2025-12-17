//using Octokit;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Tracker.Api.Dtos;


namespace Tracker.Api.Services
{
    public class GitHubService : IGitHubService
    {
       // private readonly GitHubClient _client;

        //to access graphql api
        private readonly HttpClient _httpClient;

        public GitHubService(HttpClient http, IConfiguration config)
        {
            _httpClient = http;
            var token = config["GitHub:Token"];

            //_client = new GitHubClient(new Octokit.ProductHeaderValue("GitHubDashboard"))
            //{
            //    Credentials = new Credentials(token)
            //};

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
            var allItems = new List<ProjectItem>();
            string? endCursor = null;
            bool hasNextPage = true;
            ProjectV2? projectInfo = null;

            Console.WriteLine($"Starting to fetch project {projectNumber} for {owner}");

            while (hasNextPage)
            {
                var query = @"
                    query GetProjectV2($owner: String!, $projectNumber: Int!, $itemsFirst: Int!, $after: String) {
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
                          items(first: $itemsFirst, after: $after) {
                            pageInfo {
                              hasNextPage
                              endCursor
                            }
                            nodes {
                              id
                              type
                              content {
                                ... on Issue { 
                                  title 
                                  url 
                                  number 
                                  state
                                  assignees(first: 10) {
                                    nodes {
                                      login
                                      name
                                      avatarUrl
                                    }
                                  }
                                }
                                ... on PullRequest { 
                                  title 
                                  url 
                                  number 
                                  merged 
                                  state
                                  assignees(first: 10) {
                                    nodes {
                                      login
                                      name
                                      avatarUrl
                                    }
                                  }
                                }
                                ... on DraftIssue { 
                                  title 
                                }
                              }
                              fieldValues(first: 30) {
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
                                  ... on ProjectV2ItemFieldUserValue {
                                    users(first: 10) {
                                      nodes {
                                        login
                                        name
                                        avatarUrl
                                      }
                                    }
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
                        owner,
                        projectNumber,
                        itemsFirst,
                        after = endCursor
                    }
                };

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var json = JsonSerializer.Serialize(body, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://api.github.com/graphql", content);
                var jsonResp = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Fetched page with cursor: {endCursor ?? "null"}, Items so far: {allItems.Count}");

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"GitHub GraphQL API HTTP error: {response.StatusCode} - {jsonResp}");

                // Check for GraphQL errors
                var errorCheck = JsonSerializer.Deserialize<GraphQLErrorResponse>(jsonResp, options);
                if (errorCheck?.errors != null && errorCheck.errors.Any())
                {
                    var errorMessages = string.Join("; ", errorCheck.errors.Select(e => e.message));
                    throw new Exception($"GitHub GraphQL API error: {errorMessages}");
                }

                var pageResult = JsonSerializer.Deserialize<GitHubProjectV2ResponseDto>(jsonResp, options);

                if (pageResult?.data?.organization?.projectV2 != null)
                {
                    // Save project info from first page
                    if (projectInfo == null)
                    {
                        projectInfo = pageResult.data.organization.projectV2;
                    }

                    // Add items from this page
                    if (pageResult.data.organization.projectV2.items?.nodes != null)
                    {
                        allItems.AddRange(pageResult.data.organization.projectV2.items.nodes);
                    }

                    // Check pagination
                    var pageInfo = pageResult.data.organization.projectV2.items?.pageInfo;
                    if (pageInfo != null)
                    {
                        hasNextPage = pageInfo.hasNextPage;
                        endCursor = pageInfo.endCursor;
                        
                        Console.WriteLine($"HasNextPage: {hasNextPage}, EndCursor: {endCursor}");
                    }
                    else
                    {
                        hasNextPage = false;
                    }
                }
                else
                {
                    hasNextPage = false;
                }

                // Safety limit to prevent infinite loops
                if (allItems.Count >= 1000)
                {
                    Console.WriteLine("Reached 1000 item limit, stopping pagination");
                    break;
                }
            }

            Console.WriteLine($"Finished fetching. Total items: {allItems.Count}");

            // Return complete result with all items
            if (projectInfo != null)
            {
                projectInfo.items = new ItemConnection
                {
                    nodes = allItems,
                    pageInfo = new PageInfo { hasNextPage = false, endCursor = null }
                };

                return new GitHubProjectV2ResponseDto
                {
                    data = new Data
                    {
                        organization = new Dtos.Organization
                        {
                            projectV2 = projectInfo
                        }
                    }
                };
            }

            return null;
        }

        // Helper classes for error checking
        private class GraphQLErrorResponse
        {
            public List<GraphQLError>? errors { get; set; }
        }

        private class GraphQLError
        {
            public string? message { get; set; }
            public string? type { get; set; }
        }



        //REST API METHODS
       
        //public async Task<List<string>> GetUserOrganizationsAsync()
        //{
        //    var orgs = await _client.Organization.GetAllForCurrent();

        //    return orgs.Select(o => o.Login).ToList();
        //}
        
    }
}
