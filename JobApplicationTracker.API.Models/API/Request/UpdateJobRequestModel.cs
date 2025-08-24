using JobApplicationTracker.Common.Enums;
using System;

public class UpdateJobRequestModel
{
    public Guid JobId { get; set; }
    public string Company { get; set; } = "";
    public string Position { get; set; } = "";
    public JobStatus Status { get; set; } = JobStatus.Applied;
    public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}


