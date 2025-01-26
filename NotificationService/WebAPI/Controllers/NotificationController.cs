using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Features.Commands;
using NotificationService.Application.Features.Queries;
using NotificationService.Infrastructure.Cache;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Paging;
using Microsoft.Extensions.Logging;

namespace NotificationService.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICacheService _cacheService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            IMediator mediator,
            ICacheService cacheService,
            ILogger<NotificationController> logger)
        {
            _mediator = mediator;
            _cacheService = cacheService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNotifications([FromQuery] PageRequest pageRequest)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("userId")?.Value);
                var cacheKey = $"notifications_user_{userId}_page_{pageRequest.Page}";

                var cachedNotifications = await _cacheService.GetAsync<PaginatedList<Notification>>(cacheKey);
                if (cachedNotifications != null)
                {
                    return Ok(cachedNotifications);
                }

                var query = new GetUserNotificationsQuery
                {
                    UserId = userId,
                    PageRequest = pageRequest
                };
                var result = await _mediator.Send(query);

                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(1));
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user notifications");
                return StatusCode(500, new { message = "Error retrieving notifications" });
            }
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("userId")?.Value);
                var cacheKey = $"notifications_unread_count_{userId}";

                var cachedCount = await _cacheService.GetAsync<int>(cacheKey);
                if (cachedCount != default)
                {
                    return Ok(new { count = cachedCount });
                }

                var query = new GetUnreadNotificationCountQuery { UserId = userId };
                var count = await _mediator.Send(query);

                await _cacheService.SetAsync(cacheKey, count, TimeSpan.FromMinutes(1));
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unread notification count");
                return StatusCode(500, new { message = "Error retrieving unread count" });
            }
        }

        [HttpPost("{id}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("userId")?.Value);
                var command = new MarkNotificationAsReadCommand
                {
                    NotificationId = id,
                    UserId = userId
                };

                await _mediator.Send(command);

                // Invalidate related caches
                await _cacheService.RemoveAsync($"notifications_user_{userId}");
                await _cacheService.RemoveAsync($"notifications_unread_count_{userId}");

                return Ok(new { message = "Notification marked as read" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                return StatusCode(500, new { message = "Error marking notification as read" });
            }
        }

        [HttpPost("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("userId")?.Value);
                var command = new MarkAllNotificationsAsReadCommand { UserId = userId };

                await _mediator.Send(command);

                // Invalidate related caches
                await _cacheService.RemoveAsync($"notifications_user_{userId}");
                await _cacheService.RemoveAsync($"notifications_unread_count_{userId}");

                return Ok(new { message = "All notifications marked as read" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                return StatusCode(500, new { message = "Error marking all notifications as read" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("userId")?.Value);
                var command = new DeleteNotificationCommand
                {
                    NotificationId = id,
                    UserId = userId
                };

                await _mediator.Send(command);

                // Invalidate related caches
                await _cacheService.RemoveAsync($"notifications_user_{userId}");
                await _cacheService.RemoveAsync($"notifications_unread_count_{userId}");

                return Ok(new { message = "Notification deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification");
                return StatusCode(500, new { message = "Error deleting notification" });
            }
        }
    }
}