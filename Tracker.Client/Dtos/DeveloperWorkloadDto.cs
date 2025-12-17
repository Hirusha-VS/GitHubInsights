namespace Tracker.Client.Dtos
{
    public class DeveloperWorkloadDto
    {
        public string Login { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public int TotalTasks { get; set; }
        public int DoneTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int InReviewTasks { get; set; }
        public int BacklogTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int CompletionPercentage { get; set; }
        public int DonePercentage { get; set; }
        public int InProgressPercentage { get; set; }
        public int InReviewPercentage { get; set; }
        public int BacklogPercentage { get; set; }
         
    }
}