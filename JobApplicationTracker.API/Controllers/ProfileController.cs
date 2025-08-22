using JobApplicationTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.API.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
