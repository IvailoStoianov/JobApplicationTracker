using JobApplicationTracker.Data.Data;
using JobApplicationTracker.Data.Models;
using JobApplicationTracker.Data.Repository;
using JobApplicationTracker.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Data.Repository
{
    public class UserRepository : BaseRepository<ApplicationUser, Guid>, IUserRepository
    {
        public UserRepository(JobApplicationTrackerDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            return await this.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<ApplicationUser> GetByUsernameAsync(string username)
        {
            return await this.FirstOrDefaultAsync(u => u.UserName == username);
        }
    }
}
