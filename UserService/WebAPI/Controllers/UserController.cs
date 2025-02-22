using MassTransit;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models.Messages;
using UserService.Application.Common.Interface.Services;
using UserService.Application.DTO.Requests;
using UserService.Domain.Entities;
using UserService.Domain.Paging;
using UserService.Infrastructure.Cache;

namespace UserService.WebAPI.Controllers
{
   // [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<UserController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public UserController(
            IUserService userService,
            ICacheService cacheService,
            ILogger<UserController> logger,
            IPublishEndpoint publishEndpoint)
        {
            _userService = userService;
            _cacheService = cacheService;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("userId")?.Value);
                var cacheKey = $"user_profile_{userId}";

                // Try to get from cache first
                var cachedUser = await _cacheService.GetAsync<User>(cacheKey);
                if (cachedUser != null)
                {
                    return Ok(cachedUser);
                }

                var user = await _userService.GetUserByIdAsync(userId);
                await _cacheService.SetAsync(cacheKey, user, TimeSpan.FromMinutes(5));

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("submit-kyc")]
        public async Task<IActionResult> SubmitKyc([FromForm] KycSubmissionRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("userId")?.Value);
                var user = await _userService.SubmitKycAsync(userId, request);

                //  Kyc submission message for backoffice review
                await _publishEndpoint.Publish(new KycSubmittedMessage
                {
                    UserId = userId,
                    CompanyName = request.CompanyName,
                    RcNumber = request.RcNumber,
                    CompanyAddress = request.CompanyAddress,
                    Industry = request.Industry,
                    PhoneNumber = request.PhoneNumber,
                    State = request.State,
                    City = request.City,
                    SubmittedAt = DateTime.UtcNow
                });


                await _cacheService.RemoveAsync($"user_profile_{userId}");

                return Ok(new
                {
                    message = "KYC details submitted successfully and pending review",
                    user = user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting KYC for user {UserId}", User.FindFirst("userId")?.Value);
                return StatusCode(500, new { message = "Error submitting KYC details" });
            }
        }

        [HttpGet("incomplete-profiles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetIncompleteProfiles([FromQuery] PageRequest pageRequest)
        {
            try
            {
                var result = await _userService.GetIncompleteProfilesAsync(pageRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving incomplete profiles");
                return StatusCode(500, new { message = "Error retrieving incomplete profiles" });
            }
        }

        [HttpGet("{userId}/names")]
        public async Task<IActionResult> GetUserNames(Guid userId)
        {
            try
            {
                var user = await _userService.GetUserNamesByIdAsync(userId);
                if (user == null)
                {
                    return NotFound($"User with ID {userId} not found");
                }

                return Ok(new
                {
                    FirstName = user.Value.FirstName,
                    LastName = user.Value.LastName,
                    //IsActive = true,
                    //Role = "MSME" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user names for ID: {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving user names");
            }
        }
    }
}