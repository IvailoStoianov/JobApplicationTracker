using JobApplicationTracker.Data.Data;
using JobApplicationTracker.Data.Models;
using JobApplicationTracker.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Repository
{
    public class JobRepository : BaseRepository<Job, Guid>, IJobRepository
    {
        private readonly JobApplicationTrackerDbContext _dbContext;

        public JobRepository(JobApplicationTrackerDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Job>> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.Set<Job>()
                .Where(j => j.ApplicationUserId == userId)
                .ToListAsync();
        }

        public IQueryable<Job> QueryByUser(Guid userId)
        {
            return _dbContext.Set<Job>().Where(j => j.ApplicationUserId == userId);
        }
    }
}


