namespace Tracker.Api.Dtos.ChartDtos
{
    public class DeveloperWorkloadDto
    {
        public string Developer { get; set; }
        public int Total { get; set; }
        public int Completed { get; set; }
        public int Pending { get; set; }
    }
}
