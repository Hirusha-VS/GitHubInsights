namespace Tracker.Client.Models
{
    public class OverdueInfo
    {
        public bool IsOverdue { get; set; }
        public bool IsDueSoon { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}