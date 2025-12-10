using Tracker.Client.Dtos;

namespace Tracker.Client.Extensions
{
    public static class ProjectItemExtensions
    {
        public static List<Assignee> GetAllAssignees(this ProjectItem item)
        {
            var assigneeList = new List<Assignee>();

            // Method 1: Get assignees from content (Issue/PR assignees from GitHub)
            if (item.content?.assignees?.nodes != null && item.content.assignees.nodes.Any())
            {
                assigneeList.AddRange(item.content.assignees.nodes);
            }

            // Method 2: Get assignees from ProjectV2 custom field values
            var userFields = item.fieldValues?.nodes?
                .Where(f => f.__typename == "ProjectV2ItemFieldUserValue" && f.users?.nodes != null && f.users.nodes.Any())
                .ToList();

            if (userFields != null && userFields.Any())
            {
                foreach (var userField in userFields)
                {
                    if (userField.users?.nodes != null)
                    {
                        assigneeList.AddRange(userField.users.nodes.Select(u => new Assignee
                        {
                            login = u.login,
                            name = u.name,
                            avatarUrl = u.avatarUrl
                        }));
                    }
                }
            }

            // Remove duplicates by login
            return assigneeList
                .Where(a => !string.IsNullOrEmpty(a.login))
                .GroupBy(a => a.login)
                .Select(g => g.First())
                .ToList();
        }

        public static string GetStatus(this ProjectItem item)
        {
            var statusField = item.fieldValues?.nodes?
                .FirstOrDefault(f => f.field?.name == "Status" && !string.IsNullOrEmpty(f.name));

            return statusField?.name ?? string.Empty;
        }

        public static DateTime? GetEndDate(this ProjectItem item)
        {
            var endDateField = item.fieldValues?.nodes?
                .FirstOrDefault(f => f.field?.name == "End date" && !string.IsNullOrEmpty(f.date));

            if (endDateField == null || string.IsNullOrEmpty(endDateField.date))
            {
                return null;
            }

            if (DateTime.TryParse(endDateField.date, out var endDate))
            {
                return endDate;
            }

            return null;
        }

        public static bool IsOverdue(this ProjectItem item)
        {
            var endDate = item.GetEndDate();
            if (!endDate.HasValue) return false;

            return endDate.Value.Date < DateTime.Today;
        }

        public static bool IsDueToday(this ProjectItem item)
        {
            var endDate = item.GetEndDate();
            if (!endDate.HasValue) return false;

            return endDate.Value.Date == DateTime.Today;
        }

        public static string GetItemTypeLabel(this ProjectItem item)
        {
            return item.type?.ToLower() switch
            {
                "issue" => "Issue",
                "pull_request" => "PR",
                "draft_issue" => "Draft",
                _ => "Item"
            };
        }

        public static string GetItemTypeBadgeClass(this ProjectItem item)
        {
            return item.type?.ToLower() switch
            {
                "issue" => "issue",
                "pull_request" => "pr",
                "draft_issue" => "draft",
                _ => "draft"
            };
        }
    }
}