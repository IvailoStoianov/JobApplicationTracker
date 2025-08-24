using JobApplicationTracker.Common.Enums;

public class UserJobResponseModel
{
        public Guid JobId { get; set; }
        public string Company { get; set; } = "";
        public string Position { get; set; } = "";
        public JobStatus Status { get; set; } = JobStatus.Applied;
        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
        public decimal? Salary { get; set; }
        public string? Contact { get; set; }
}