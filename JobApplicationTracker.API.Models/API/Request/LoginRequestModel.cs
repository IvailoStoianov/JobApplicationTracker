﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.API.Models.API.Request
{
    public class LoginRequestModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
