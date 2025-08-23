using JobApplicationTracker.API.Models.API.Request;
using JobApplicationTracker.API.Models.API.Response;
using JobApplicationTracker.Data.Models;
using JobApplicationTracker.Data.Repository.Interfaces;
using JobApplicationTracker.Services.Helpers;
using JobApplicationTracker.Services.Interfaces;

namespace JobApplicationTracker.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;

        public AccountService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<RegisterResponseModel?> RegisterAsync(RegisterRequestModel request)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.Username))
            {
                return null;
            }

            // Check if user already exists by email or username
            var existingByEmail = await _userRepository.GetByEmailAsync(request.Email);
            if (existingByEmail != null)
            {
                return null;
            }

            var existingByUsername = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingByUsername != null)
            {
                return null;
            }

            PasswordHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new ApplicationUser
            {
                UserName = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            await _userRepository.AddAsync(user);

            return new RegisterResponseModel
            {
                UserId = user.Id.ToString(),
                Email = user.Email
            };
        }
    }
}
