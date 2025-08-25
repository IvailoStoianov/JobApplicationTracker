using System;
using System.Threading.Tasks;
using JobApplicationTracker.API.Models.API.Request;
using JobApplicationTracker.Data.Models;
using JobApplicationTracker.Data.Repository.Interfaces;
using JobApplicationTracker.Services;
using Moq;

namespace JobApplicationTracker.Tests.Unit
{
    public class AccountServiceTests
    {
        private Mock<IUserRepository> _userRepoMock = null!;
        private AccountService _service = null!;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepository>(MockBehavior.Strict);
            _service = new AccountService(_userRepoMock.Object);
        }

        [Test]
        public async Task RegisterAsync_MissingFields_ReturnsNull()
        {
            var req = new RegisterRequestModel { Email = "", Password = "", Username = "" };
            var result = await _service.RegisterAsync(req);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task RegisterAsync_EmailExists_ReturnsNull()
        {
            _userRepoMock.Setup(r => r.GetByEmailAsync("ex@test.com")).ReturnsAsync(new ApplicationUser());
            var req = new RegisterRequestModel { Email = "ex@test.com", Password = "P@ssw0rd!", Username = "user1", ConfirmPassword = "P@ssw0rd!" };
            var result = await _service.RegisterAsync(req);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task RegisterAsync_UsernameExists_ReturnsNull()
        {
            _userRepoMock.Setup(r => r.GetByEmailAsync("new@test.com")).ReturnsAsync((ApplicationUser)null);
            _userRepoMock.Setup(r => r.GetByUsernameAsync("user1")).ReturnsAsync(new ApplicationUser());
            var req = new RegisterRequestModel { Email = "new@test.com", Password = "P@ssw0rd!", Username = "user1", ConfirmPassword = "P@ssw0rd!" };
            var result = await _service.RegisterAsync(req);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task RegisterAsync_Succeeds_ReturnsResponseAndCallsAdd()
        {
            _userRepoMock.Setup(r => r.GetByEmailAsync("ok@test.com")).ReturnsAsync((ApplicationUser)null);
            _userRepoMock.Setup(r => r.GetByUsernameAsync("user2")).ReturnsAsync((ApplicationUser)null);
            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<ApplicationUser>())).Returns(Task.CompletedTask).Verifiable();

            var req = new RegisterRequestModel { Email = "ok@test.com", Password = "P@ssw0rd!", Username = "user2", ConfirmPassword = "P@ssw0rd!" };
            var res = await _service.RegisterAsync(req);

            _userRepoMock.Verify();
            Assert.That(res, Is.Not.Null);
            Assert.That(res!.Email, Is.EqualTo("ok@test.com"));
            Assert.That(Guid.TryParse(res.UserId, out _), Is.True);
        }
    }
}


