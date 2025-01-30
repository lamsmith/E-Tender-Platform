using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthService.Application.Common.Interface.Services;
using AuthService.Application.DTO.Requests;
using AuthService.Application.Features.Commands;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using MediatR;

namespace AuthService.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            IMediator mediator,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data" });
            }

            try
            {
                await _authService.RegisterAsync(request);
                return Ok(new { message = "Registration successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data" });
            }

            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                await _authService.LogoutAsync(userId);
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("validate")]
        public IActionResult ValidateToken()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var email = User.FindFirst("email")?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                return Ok(new
                {
                    userId,
                    email,
                    role,
                    isValid = true
                });
            }
            catch
            {
                return Unauthorized(new { message = "Invalid token" });
            }
        }

        [HttpPost("staff")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateStaffUser([FromBody] CreateStaffUserRequest request)
        {
            try
            {
                var command = new CreateStaffUserCommand
                {
                    Email = request.Email,
                    Role = request.SelectedRole
                };

                var result = await _mediator.Send(command);
                return Ok(new { UserId = result.UserId, TempPassword = result.TempPassword });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff user");
                return StatusCode(500, new { message = "Error creating staff user" });
            }
        }

        [HttpPost("staff/{userId}/notify")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> NotifyStaffUser(Guid userId)
        {
            try
            {
                var command = new SendStaffWelcomeEmailCommand { UserId = userId };
                await _mediator.Send(command);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending welcome email");
                return StatusCode(500, new { message = "Error sending welcome email" });
            }
        }

        [HttpPost("staff/reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var command = new ResetPasswordCommand
                {
                    UserId = request.UserId,
                    CurrentPassword = request.CurrentPassword,
                    NewPassword = request.NewPassword
                };

                await _mediator.Send(command);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                return StatusCode(500, new { message = "Error resetting password" });
            }
        }

        //    [HttpPost("refresh-token")]
        //    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        //    {
        //        // TODO: Implement refresh token logic
        //        throw new NotImplementedException();
        //    }

        //    public class RefreshTokenRequest
        //    {
        //        public string RefreshToken { get; set; }
        //    }
    }
}