using System;
using System.Security.Claims;
using JobApplicationTracker.API.Infrastructure.Extentions;

namespace JobApplicationTracker.Tests.Unit
{
    public class ClaimsPrincipalExtensionsTests
    {
        [Test]
        public void GetUserId_ReturnsGuid_WhenPresent()
        {
            var id = Guid.NewGuid();
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString())
            }, "test"));

            var result = principal.GetUserId();
            Assert.That(result, Is.EqualTo(id));
        }

        [Test]
        public void GetUserId_ReturnsNull_WhenMissing()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity());
            var result = principal.GetUserId();
            Assert.That(result, Is.Null);
        }
    }
}


