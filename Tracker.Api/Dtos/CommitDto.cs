namespace Tracker.Api.Dtos
{
    public class CommitDto
    {
        public string Sha { get; set; }
        public string Message { get; set; }
        public string AuthorName { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
