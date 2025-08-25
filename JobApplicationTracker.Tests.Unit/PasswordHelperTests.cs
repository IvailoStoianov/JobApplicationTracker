using JobApplicationTracker.Services.Helpers;

namespace JobApplicationTracker.Tests.Unit
{
    public class PasswordHelperTests
    {
        [Test]
        public void CreateAndVerifyPassword_Works()
        {
            const string pwd = "P@ssw0rd!";
            PasswordHelper.CreatePasswordHash(pwd, out var hash, out var salt);
            Assert.That(hash, Is.Not.Null);
            Assert.That(salt, Is.Not.Null);
            var ok = PasswordHelper.VerifyPasswordHash(pwd, hash, salt);
            Assert.That(ok, Is.True);
            var bad = PasswordHelper.VerifyPasswordHash("wrong", hash, salt);
            Assert.That(bad, Is.False);
        }
    }
}


