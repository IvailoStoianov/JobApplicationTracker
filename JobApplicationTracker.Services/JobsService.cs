using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobApplicationTracker.Data.Models;
using JobApplicationTracker.Services.Interfaces;
using JobApplicationTracker.Data.Repository.Interfaces;
using JobApplicationTracker.API.Models.API.Response;
using JobApplicationTracker.API.Models.API.Request;
using JobApplicationTracker.API.Infrastructure.Common.Constants;

namespace JobApplicationTracker.Services
{
    public class JobsService : IJobsService
    {
        private readonly IJobRepository _jobRepository;
        public JobsService(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<AllUsersJobsResponseModel> GetAllUserJobs(Guid userId, GetUserJobsQuery? query = null)
        {
            var q = _jobRepository.QueryByUser(userId);
            if (query != null)
            {
                if (query.Status.HasValue)
                {
                    q = q.Where(j => j.Status == query.Status.Value);
                }
                if (query.From.HasValue)
                {
                    q = q.Where(j => j.ApplicationDate >= query.From.Value);
                }
                if (query.To.HasValue)
                {
                    q = q.Where(j => j.ApplicationDate <= query.To.Value);
                }
            }

            var total = q.Count();
            var page = Math.Max(1, query?.Page ?? 1);
            var pageSize = Math.Clamp(query?.PageSize ?? 20, 1, 200);
            var items = q
                .OrderByDescending(j => j.ApplicationDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new AllUsersJobsResponseModel
            {
                Jobs = items.Select(j => new UserJobResponseModel { JobId = j.Id, Company = j.Company, Position = j.Position, Status = j.Status, ApplicationDate = j.ApplicationDate, LastUpdated = j.LastUpdated, Notes = j.Notes, Salary = j.Salary, Contact = j.Contact }).ToList(),
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }

        public async Task<UserJobResponseModel> CreateJob(JobRequestModel model, Guid userId)
        {
            var job = new Job { Company = model.Company, Position = model.Position, Status = model.Status, ApplicationDate = model.ApplicationDate, Notes = model.Notes, Salary = model.Salary, Contact = model.Contact, ApplicationUserId = userId };
            job.LastUpdated = DateTime.UtcNow;
            await _jobRepository.AddAsync(job);
            return new UserJobResponseModel { JobId = job.Id, Company = job.Company, Position = job.Position, Status = job.Status, ApplicationDate = job.ApplicationDate, LastUpdated = job.LastUpdated, Notes = job.Notes, Salary = job.Salary, Contact = job.Contact };
        }

        public async Task<UserJobResponseModel> UpdateJob(UpdateJobRequestModel model, Guid userId)
        {
            var job = await _jobRepository.GetByIdAsync(model.JobId);
            if (job == null) throw new Exception(ApiMessages.Jobs.JobNotFound);
            if (job.ApplicationUserId != userId) throw new Exception(ApiMessages.Jobs.Forbidden);
            job.Company = model.Company;
            job.Position = model.Position;
            job.Status = model.Status;
            job.ApplicationDate = model.ApplicationDate;
            job.Notes = model.Notes;
            job.Salary = model.Salary;
            job.Contact = model.Contact;
            job.LastUpdated = DateTime.UtcNow;
            await _jobRepository.UpdateAsync(job);
            return new UserJobResponseModel { JobId = job.Id, Company = job.Company, Position = job.Position, Status = job.Status, ApplicationDate = job.ApplicationDate, LastUpdated = job.LastUpdated, Notes = job.Notes, Salary = job.Salary, Contact = job.Contact };
        }

        public async Task DeleteJob(Guid id, Guid userId)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null) throw new Exception(ApiMessages.Jobs.JobNotFound);
            if (job.ApplicationUserId != userId) throw new Exception(ApiMessages.Jobs.Forbidden);
            await _jobRepository.DeleteAsync(job);
        }

    }
}
