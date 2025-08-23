using JobApplicationTracker.API.Infrastructure.Common;
using JobApplicationTracker.API.Infrastructure.Common.Constants;
using JobApplicationTracker.API.Models.API.Request;
using JobApplicationTracker.API.Models.API.Response;
using JobApplicationTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IJwtService _jwtService;

        public AccountController(IAccountService accountService, IJwtService jwtService)
        {
            _accountService = accountService;
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<ApiResponse<LoginResponseModel>>> Login(LoginRequestModel request)
        {
            var result = await _jwtService.Authenticate(request);

            if (result == null)
            {
                return Unauthorized(ApiResponse<LoginResponseModel>.Fail(ApiMessages.Auth.LoginFailed));
            }

            return Ok(ApiResponse<LoginResponseModel>.Ok(result, ApiMessages.Auth.LoginSuccess));
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<ApiResponse<RegisterResponseModel>>> Register(RegisterRequestModel request)
        {
            var result = await _accountService.RegisterAsync(request);

            if (result == null)
            {
                return BadRequest(ApiResponse<RegisterResponseModel>.Fail(ApiMessages.Auth.RegisterFailed));
            }

            return Ok(ApiResponse<RegisterResponseModel>.Ok(result, ApiMessages.Auth.RegisterSuccess));
        }
    }
}
