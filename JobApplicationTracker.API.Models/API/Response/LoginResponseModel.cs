﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.API.Models.API.Response
{
    public class LoginResponseModel
    {
        public string? Email { get; set; }
        public string? AccessToken { get; set; }
        public int ExpiresInSeconds { get; set; }
    }
}
