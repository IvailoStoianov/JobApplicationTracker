using System;
using System.Linq;
using System.Security.Claims;

namespace JobApplicationTracker.API.Infrastructure.Extentions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetUserId(this ClaimsPrincipal user)
        {
            if (user == null)
            {
                return null;
            }

            var idValue = user.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(idValue, out var id))
            {
                return id;
            }

            return null;
        }
    }
}


