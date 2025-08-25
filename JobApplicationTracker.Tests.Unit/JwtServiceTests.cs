using System;
using System.Threading.Tasks;
using JobApplicationTracker.API.Models.API.Request;
using JobApplicationTracker.Data.Models;
using JobApplicationTracker.Data.Repository.Interfaces;
using JobApplicationTracker.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace JobApplicationTracker.Tests.Unit
{
    public class JwtServiceTests
    {
        private IConfiguration _config = null!;
        private Mock<IUserRepository> _userRepoMock = null!;
        private JwtService _service = null!;

        [SetUp]
        public void Setup()
        {
            var dict = new Dictionary<string, string?>
            {
                ["JwtConfig:Issuer"] = "test-issuer",
                ["JwtConfig:Audience"] = "test-audience",
                ["JwtConfig:Key"] = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
                ["JwtConfig:TokenValidityMins"] = "60",
            };
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(dict)
                .Build();
            _userRepoMock = new Mock<IUserRepository>();
            _service = new JwtService(_config, _userRepoMock.Object);
        }

        [Test]
        public async Task Authenticate_InvalidCredentials_ReturnsNull()
        {
            var req = new LoginRequestModel { Email = "x@test.com", Password = "wrong" };
            _userRepoMock.Setup(r => r.GetByEmailAsync(req.Email)).ReturnsAsync((ApplicationUser)null);
            var result = await _service.Authenticate(req);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Authenticate_ValidCredentials_ReturnsToken()
        {
            var salt = new byte[16];
            var hash = new byte[32];
            JobApplicationTracker.Services.Helpers.PasswordHelper.CreatePasswordHash("P@ssw0rd!", out hash, out salt);
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "x", Email = "x@test.com", PasswordHash = hash, PasswordSalt = salt };
            _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

            var res = await _service.Authenticate(new LoginRequestModel { Email = user.Email, Password = "P@ssw0rd!" });

            Assert.That(res, Is.Not.Null);
            Assert.That(string.IsNullOrWhiteSpace(res!.AccessToken), Is.False);
            Assert.That(res.Email, Is.EqualTo(user.UserName));
            Assert.That(res.ExpiresInSeconds, Is.GreaterThan(0));
        }
    }
}


