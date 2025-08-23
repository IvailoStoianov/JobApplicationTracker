using JobApplicationTracker.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Repository.Interfaces
{
    public interface IUserRepository : IRepository<ApplicationUser, Guid>
    {
        public Task<ApplicationUser> GetByEmailAsync(string email);
        public Task<ApplicationUser> GetByUsernameAsync(string username);
    }
}
