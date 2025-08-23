using JobApplicationTracker.API.Models.API.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Services.Interfaces
{
    public interface IAccountService
    {
        Task<RegisterResponseModel> RegisterAsync(JobApplicationTracker.API.Models.API.Request.RegisterRequestModel request);
    }
}
