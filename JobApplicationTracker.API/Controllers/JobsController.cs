using JobApplicationTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JobApplicationTracker.API.Infrastructure.Extentions;
using JobApplicationTracker.API.Infrastructure.Common.Constants;
using JobApplicationTracker.API.Infrastructure.Common;

namespace JobApplicationTracker.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : Controller
    {
        private readonly IJobsService _jobsService;
        public JobsController(IJobsService jobsService)
        {
            _jobsService = jobsService;
        }
        
        [HttpGet("GetAllUserJobs")]
        public async Task<ActionResult<ApiResponse<AllUsersJobsResponseModel>>> GetAllUserJobs([FromQuery] GetUserJobsQuery query)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized(ApiResponse<AllUsersJobsResponseModel>.Fail(ApiMessages.Jobs.Unauthorized));
            var jobs = await _jobsService.GetAllUserJobs(userId.Value, query);
            return Ok(ApiResponse<AllUsersJobsResponseModel>.Ok(jobs, ApiMessages.Jobs.JobsFetched));
        }

        [HttpPost("CreateJob")]
        public async Task<ActionResult<ApiResponse<UserJobResponseModel>>> CreateJob(JobRequestModel model)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized(ApiResponse<UserJobResponseModel>.Fail(ApiMessages.Jobs.Unauthorized));
            var newJob = await _jobsService.CreateJob(model, userId.Value);
            return Ok(ApiResponse<UserJobResponseModel>.Ok(newJob, ApiMessages.Jobs.JobCreated));
        }

        [HttpPut("UpdateJob")]
        public async Task<ActionResult<ApiResponse<UserJobResponseModel>>> UpdateJob(UpdateJobRequestModel model)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized(ApiResponse<UserJobResponseModel>.Fail(ApiMessages.Jobs.Unauthorized));
            try
            {
                var updatedJob = await _jobsService.UpdateJob(model, userId.Value);
                return Ok(ApiResponse<UserJobResponseModel>.Ok(updatedJob, ApiMessages.Jobs.JobUpdated));
            }
            catch (Exception ex) when (ex.Message == ApiMessages.Jobs.JobNotFound)
            {
                return NotFound(ApiResponse<UserJobResponseModel>.Fail(ApiMessages.Jobs.JobNotFound));
            }
            catch (Exception ex) when (ex.Message == ApiMessages.Jobs.Forbidden)
            {
                return Forbid();
            }
        }

        [HttpDelete("DeleteJob")]
        public async Task<ActionResult<ApiResponse<UserJobResponseModel>>> DeleteJob(Guid id)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized(ApiResponse<UserJobResponseModel>.Fail(ApiMessages.Jobs.Unauthorized));
            try
            {
                await _jobsService.DeleteJob(id, userId.Value);
                return Ok(ApiResponse<UserJobResponseModel>.Ok(null, ApiMessages.Jobs.JobDeleted));
            }
            catch (Exception ex) when (ex.Message == ApiMessages.Jobs.JobNotFound)
            {
                return NotFound(ApiResponse<UserJobResponseModel>.Fail(ApiMessages.Jobs.JobNotFound));
            }
            catch (Exception ex) when (ex.Message == ApiMessages.Jobs.Forbidden)
            {
                return Forbid();
            }
        }
        
    }
}
