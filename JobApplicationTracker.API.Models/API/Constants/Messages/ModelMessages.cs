using JobApplicationTracker.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.API.Models.API.Constants.Messages
{
    public static class ModelMessages
    {
        public static class Auth
        {
            public const string PasswordIsTooShort = "Password must be at least {0} characters long.";
            public const string PasswordsDoNotMatch = "Passwords do not match.";
        }
    }
}
