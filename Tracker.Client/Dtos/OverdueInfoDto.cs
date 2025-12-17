namespace Tracker.Client.Dtos
{
    public class OverdueInfoDto
    {
        public bool IsOverdue { get; set; }
        public bool IsDueSoon { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}