namespace Tracker.Api.Dtos
{
    public class PullRequestDto
    {
        public int Number { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? Url { get; set; }
        public string? State { get; set; }
    }
}
