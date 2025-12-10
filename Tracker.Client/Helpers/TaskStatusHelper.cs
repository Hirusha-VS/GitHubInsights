using Tracker.Client.Dtos;
using Tracker.Client.Extensions;

namespace Tracker.Client.Helpers
{
    public static class TaskStatusHelper
    {
        public static List<string> GetAllStatuses(List<ProjectItem> items)
        {
            if (items == null) return new List<string>();

            return items
                .SelectMany(item => item.fieldValues?.nodes ?? new List<FieldValue>())
                .Where(f => f.field?.name == "Status" && !string.IsNullOrEmpty(f.name))
                .Select(f => f.name!)
                .Distinct()
                .OrderBy(s => s)
                .ToList();
        }

        public static int GetItemsByStatus(List<ProjectItem> items, string status)
        {
            if (items == null) return 0;
            return items.Count(item =>
                item.fieldValues?.nodes?.Any(f =>
                    f.field?.name == "Status" && f.name == status) == true);
        }

        public static int GetItemsWithoutStatus(List<ProjectItem> items)
        {
            if (items == null) return 0;
            return items.Count(item =>
                item.fieldValues?.nodes?.Any(f => f.field?.name == "Status") != true);
        }

        public static List<ProjectItem> GetItemsForColumn(List<ProjectItem> items, string? status)
        {
            if (items == null) return new List<ProjectItem>();

            if (status == null)
            {
                // Unassigned items
                return items
                    .Where(item => item.fieldValues?.nodes?.Any(f => f.field?.name == "Status") != true)
                    .ToList();
            }

            return items
                .Where(item => item.fieldValues?.nodes?.Any(f =>
                    f.field?.name == "Status" && f.name == status) == true)
                .ToList();
        }

        public static int GetOpenPullRequests(List<ProjectItem> items)
        {
            if (items == null) return 0;
            return items.Count(i =>
                i.type == "PULL_REQUEST" &&
                i.content?.state?.ToLower() == "open");
        }

        public static int GetMergedPullRequests(List<ProjectItem> items)
        {
            if (items == null) return 0;
            return items.Count(i =>
                i.type == "PULL_REQUEST" &&
                i.content?.merged == true);
        }

        public static int GetOpenIssues(List<ProjectItem> items)
        {
            if (items == null) return 0;
            return items.Count(i =>
                i.type == "ISSUE" &&
                i.content?.state?.ToLower() == "open");
        }
    }
}