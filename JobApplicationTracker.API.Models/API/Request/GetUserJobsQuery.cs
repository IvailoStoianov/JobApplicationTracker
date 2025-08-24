using JobApplicationTracker.Common.Enums;
using System;

public class GetUserJobsQuery
{
    public JobStatus? Status { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}


