using JobApplicationTracker.Common.Enums;

public class JobRequestModel
{
    public string Company { get; set; } = "";
    public string Position { get; set; } = "";
    public JobStatus Status { get; set; } = JobStatus.Applied;
    public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}