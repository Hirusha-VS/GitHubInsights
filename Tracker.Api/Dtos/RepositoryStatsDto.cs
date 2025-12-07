using System.Globalization;

namespace Tracker.Api.Dtos
{
    public class RepositoryStatsDto
    {
        public string? Name { get; set; }
        public int Stars { get; set; }
        public int Forks { get; set; }
        public int OpenIssues { get; set; }
        public int OpenPRs { get; set; }
        public string? Language { get; set; }
        public OwnerDto? Owner { get; internal set; }
    }
}
