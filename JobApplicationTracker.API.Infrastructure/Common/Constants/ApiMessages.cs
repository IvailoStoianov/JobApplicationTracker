using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.API.Infrastructure.Common.Constants
{
    public static class ApiMessages
    {
        public static class Auth
        {
            public const string LoginSuccess = "Login successful";
            public const string LoginFailed = "Invalid username or password.";
            public const string RegisterSuccess = "User registered successfully";
            public const string RegisterFailed = "Registration failed. User may already exist.";
        }
        
        public static class Jobs
        {
            public const string JobNotFound = "Job not found";
            public const string JobCreated = "Job created successfully";
        }
        
    }
}
