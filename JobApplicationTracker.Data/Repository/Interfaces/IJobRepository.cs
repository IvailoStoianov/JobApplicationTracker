using JobApplicationTracker.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Repository.Interfaces
{
    public interface IJobRepository : IRepository<Job, Guid>
    {
        Task<IEnumerable<Job>> GetByUserIdAsync(Guid userId);
        IQueryable<Job> QueryByUser(Guid userId);
    }
}


