using Tracker.Client.Dtos;

namespace Tracker.Client.Models
{
    public class TaskAnalytics
    {
        public int TotalItems { get; set; }
        public int DueToday { get; set; }
        public int Overdue { get; set; }
        public int InProgress { get; set; }
        public int Done { get; set; }
        public List<ProjectItem> TasksDueToday { get; set; } = new();
        public List<ProjectItem> OverdueTasks { get; set; } = new();
    }
}