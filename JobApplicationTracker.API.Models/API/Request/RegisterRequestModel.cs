using JobApplicationTracker.API.Models.API.Constants.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.API.Models.API.Request
{
    public class RegisterRequestModel
    {
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        [Compare(nameof(Password), ErrorMessage = ModelMessages.Auth.PasswordsDoNotMatch)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
