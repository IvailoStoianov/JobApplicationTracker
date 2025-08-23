using JobApplicationTracker.API.Models.API.Request;
using JobApplicationTracker.API.Models.API.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Services.Interfaces
{
    public interface IJwtService
    {
        public Task<LoginResponseModel> Authenticate(LoginRequestModel request);
    }
}
