namespace Tracker.Api.Dtos
{
    public class RepositoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public int Stars { get; set; }
        public int Forks { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public int OpenIssues { get; set; }
        public OwnerDto Owner { get; internal set; }
    }
}
