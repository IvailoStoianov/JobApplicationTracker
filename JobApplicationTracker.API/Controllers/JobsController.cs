using JobApplicationTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.API.Controllers
{
    [Authorize]
    [ApiController]
    public class JobsController : Controller
    {
        private readonly IJobsService _jobsService;
        public JobsController(IJobsService jobsService)
        {
            _jobsService = jobsService;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
