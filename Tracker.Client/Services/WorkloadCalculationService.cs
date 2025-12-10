using Tracker.Client.Dtos;
using Tracker.Client.Extensions;
using Tracker.Client.Helpers;
using Tracker.Client.Models;

namespace Tracker.Client.Services
{
    public class WorkloadCalculationService
    {
        public List<DeveloperWorkload> GetDeveloperWorkloads(ProjectV2? project)
        {
            if (project?.items?.nodes == null)
            {
                return new List<DeveloperWorkload>();
            }

            var items = project.items.nodes;

            // Get all unique assignees
            var allAssignees = items
                .SelectMany(item => item.GetAllAssignees())
                .GroupBy(a => a.login)
                .Select(g => g.First())
                .ToList();

            var workloads = new List<DeveloperWorkload>();

            foreach (var assignee in allAssignees)
            {
                var assignedItems = items
                    .Where(item => item.GetAllAssignees().Any(a => a.login == assignee.login))
                    .ToList();

                var doneTasks = assignedItems.Count(item =>
                    item.fieldValues?.nodes?.Any(f => f.field?.name == "Status" && f.name == "Done") == true);

                var inProgressTasks = assignedItems.Count(item =>
                    item.fieldValues?.nodes?.Any(f => f.field?.name == "Status" && f.name == "In progress") == true);

                var inReviewTasks = assignedItems.Count(item =>
                    item.fieldValues?.nodes?.Any(f => f.field?.name == "Status" && f.name == "In review") == true);

                var backlogTasks = assignedItems.Count(item =>
                    item.fieldValues?.nodes?.Any(f => f.field?.name == "Status" && f.name == "Backlog") == true);

                var overdueTasks = assignedItems.Count(item => DateHelper.GetOverdueInfo(item).IsOverdue);

                var totalTasks = assignedItems.Count;

                workloads.Add(new DeveloperWorkload
                {
                    Login = assignee.login,
                    Name = assignee.name ?? assignee.login,
                    AvatarUrl = assignee.avatarUrl,
                    TotalTasks = totalTasks,
                    DoneTasks = doneTasks,
                    InProgressTasks = inProgressTasks,
                    InReviewTasks = inReviewTasks,
                    BacklogTasks = backlogTasks,
                    OverdueTasks = overdueTasks,
                    CompletionPercentage = PercentageCalculator.CalculateCompletionRate(doneTasks, totalTasks),
                    DonePercentage = PercentageCalculator.CalculateDistribution(doneTasks, totalTasks),
                    InProgressPercentage = PercentageCalculator.CalculateDistribution(inProgressTasks, totalTasks),
                    InReviewPercentage = PercentageCalculator.CalculateDistribution(inReviewTasks, totalTasks),
                    BacklogPercentage = PercentageCalculator.CalculateDistribution(backlogTasks, totalTasks)
                });
            }

            return workloads;
        }
    }
}