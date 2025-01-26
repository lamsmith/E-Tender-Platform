using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Common.Interface.Services;
using UserService.Application.DTO.Requests;
using UserService.Domain.Entities;
using UserService.Infrastructure.Cache;

namespace UserService.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICacheService _cacheService;

        public UserController(IUserService userService, ICacheService cacheService)
        {
            _userService = userService;
            _cacheService = cacheService;
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

        //    [HttpPost("change-password")]
        //    public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordRequestModel request)
        //    {
        //        try
        //        {
        //            var userId = Guid.Parse(User.FindFirst("userId")?.Value);
        //            await _userService.ChangePasswordAsync(userId, request);
        //            return Ok(new { message = "Password changed successfully" });
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(new { message = ex.Message });
        //        }
        //    }

        //    [HttpGet("statistics")]
        //    public async Task<IActionResult> GetUserStatistics()
        //    {
        //        try
        //        {
        //            var userId = Guid.Parse(User.FindFirst("userId")?.Value);
        //            var cacheKey = $"user_statistics_{userId}";

        //            // Try to get from cache
        //            var cachedStats = await _cacheService.GetAsync<UserStatistics>(cacheKey);
        //            if (cachedStats != null)
        //            {
        //                return Ok(cachedStats);
        //            }

        //            var statistics = new UserStatistics
        //            {
        //                RfqCreated = await _userService.GetUserRfqCreatedCountAsync(userId),
        //                BidsSubmitted = await _userService.GetUserBidsSubmittedCountAsync(userId),
        //                BidSuccessRate = await _userService.GetUserBidSuccessRateAsync(userId)
        //            };

        //            await _cacheService.SetAsync(cacheKey, statistics, TimeSpan.FromMinutes(5));

        //            return Ok(statistics);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(new { message = ex.Message });
        //        }
        //    }
        //}

        public class UserStatistics
        {
            public int RfqCreated { get; set; }
            public int BidsSubmitted { get; set; }
            public decimal BidSuccessRate { get; set; }
        }
    }
}