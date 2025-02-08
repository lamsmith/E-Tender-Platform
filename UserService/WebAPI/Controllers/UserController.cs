using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Common.Interface.Services;
using UserService.Application.DTO.Requests;
using UserService.Domain.Entities;
using UserService.Infrastructure.Cache;
using Microsoft.Extensions.Logging;
using SharedLibrary.Constants;
using SharedLibrary.Models.Messages;
using SharedLibrary.MessageBroker;

namespace UserService.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICacheService _cacheService;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            ICacheService cacheService,
            IMessagePublisher messagePublisher,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _cacheService = cacheService;
            _messagePublisher = messagePublisher;
            _logger = logger;
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

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileUpdateRequestModel request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("userId")?.Value);
                var updatedUser = await _userService.UpdateProfileAsync(userId, request);

                // Invalidate cache
                await _cacheService.RemoveAsync($"user_profile_{userId}");

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("complete-profile")]
        public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("userId")?.Value);
                var user = await _userService.UpdateProfileAsync(userId, request);

                // Publish profile completed message
                _messagePublisher.PublishMessage(MessageQueues.UserProfileCompleted, new UserProfileCompletedMessage
                {
                    UserId = userId,
                    CompletedAt = DateTime.UtcNow,
                    IsVerified = false // Will be verified by admin
                });

                // Invalidate cache
                await _cacheService.RemoveAsync($"user_profile_{userId}");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error completing profile" });
            }
        }

       
        public class UserStatistics
        {
            public int RfqCreated { get; set; }
            public int BidsSubmitted { get; set; }
            public decimal BidSuccessRate { get; set; }
        }
    }
}