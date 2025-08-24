namespace JobApplicationTracker.Services.Interfaces
{
    public interface IJobsService
    {
        Task<AllUsersJobsResponseModel> GetAllUserJobs(Guid userId, GetUserJobsQuery? query = null);
        Task<UserJobResponseModel> CreateJob(JobRequestModel model, Guid userId);
        Task<UserJobResponseModel> UpdateJob(UpdateJobRequestModel model, Guid userId);
        Task DeleteJob(Guid id, Guid userId);
    }
}
