using JobApplicationTracker.API.Models.API.Request;
using JobApplicationTracker.API.Models.API.Response;
using JobApplicationTracker.Data.Models;
using JobApplicationTracker.Data.Repository.Interfaces;
using JobApplicationTracker.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using JobApplicationTracker.Services.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
namespace JobApplicationTracker.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        public JwtService(IConfiguration configuration, IUserRepository userRepository) 
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }
        public async Task<LoginResponseModel> Authenticate(LoginRequestModel request)
        {
            if(string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                //handle empty or null username/password
                return null;
            }
            ApplicationUser userAccount = await _userRepository.GetByEmailAsync(request.Email!);
            if(userAccount == null || !PasswordHelper.VerifyPasswordHash(request.Password, userAccount.PasswordHash, userAccount.PasswordSalt))
            {
                //handle user not found or invalid password
                return null;
            }

            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];
            var key = _configuration["JwtConfig:Key"];
            var tokenValidityInMinutes = _configuration.GetValue<int>("JwtConfig:TokenValidityMins");
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityInMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, request.Email),
                    new Claim(ClaimTypes.NameIdentifier, userAccount.Id.ToString())
                }),
                Expires = tokenExpiryTimeStamp,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)), SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            return new LoginResponseModel
            {
                AccessToken = accessToken,
                ExpiresInSeconds = (int)TimeSpan.FromMinutes(tokenValidityInMinutes).TotalSeconds,
                Email = userAccount.UserName
            };
        }
    }
}
