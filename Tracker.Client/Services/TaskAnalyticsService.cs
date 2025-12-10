using Tracker.Client.Dtos;
using Tracker.Client.Extensions;
using Tracker.Client.Helpers;
using Tracker.Client.Models;

namespace Tracker.Client.Services
{
    public class TaskAnalyticsService
    {
        public TaskAnalytics GetAnalytics(ProjectV2? project)
        {
            if (project?.items?.nodes == null)
            {
                return new TaskAnalytics();
            }

            var items = project.items.nodes;

            return new TaskAnalytics
            {
                TotalItems = items.Count,
                DueToday = GetTasksDueToday(items).Count,
                Overdue = GetOverdueTasks(items).Count,
                InProgress = TaskStatusHelper.GetItemsByStatus(items, "In progress"),
                Done = TaskStatusHelper.GetItemsByStatus(items, "Done"),
                TasksDueToday = GetTasksDueToday(items),
                OverdueTasks = GetOverdueTasks(items)
            };
        }

        public List<ProjectItem> GetTasksDueToday(List<ProjectItem> items)
        {
            if (items == null) return new List<ProjectItem>();

            return items
                .Where(item =>
                {
                    var overdueInfo = DateHelper.GetOverdueInfo(item);
                    return overdueInfo.IsDueSoon && overdueInfo.Message == "Due today";
                })
                .ToList();
        }

        public List<ProjectItem> GetOverdueTasks(List<ProjectItem> items)
        {
            if (items == null) return new List<ProjectItem>();

            return items
                .Where(item => DateHelper.GetOverdueInfo(item).IsOverdue)
                .OrderBy(item =>
                {
                    var endDate = item.GetEndDate();
                    return endDate ?? DateTime.MaxValue;
                })
                .ToList();
        }
    }
}