using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Common.Interface.Repositories;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Paging;
using NotificationService.Infrastructure.Persistence.Context;

namespace NotificationService.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _context;
        private readonly ILogger<NotificationRepository> _logger;

        public NotificationRepository(
            NotificationDbContext context,
            ILogger<NotificationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Notification> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.Notifications.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notification with ID: {Id}", id);
                throw;
            }
        }

        public async Task<PaginatedList<Notification>> GetUserNotificationsAsync(Guid userId, PageRequest pageRequest)
        {
            try
            {
                var query = _context.Notifications
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.Timestamp);

                var count = await query.CountAsync();
                var items = await query
                    .Skip((pageRequest.Page - 1) * pageRequest.PageSize)
                    .Take(pageRequest.PageSize)
                    .ToListAsync();

                return new PaginatedList<Notification>(items, count, pageRequest.Page, pageRequest.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notifications for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            try
            {
                return await _context.Notifications
                    .CountAsync(n => n.UserId == userId && !n.IsRead);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting unread notifications for user: {UserId}", userId);
                throw;
            }
        }

        public async Task AddAsync(Notification notification)
        {
            try
            {
                notification.CreatedAt = DateTime.UtcNow;
                notification.Timestamp = DateTime.UtcNow;
                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding notification for user: {UserId}", notification.UserId);
                throw;
            }
        }

        public async Task UpdateAsync(Notification notification)
        {
            try
            {
                notification.UpdatedAt = DateTime.UtcNow;
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification: {Id}", notification.Id);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);
                if (notification != null)
                {
                    _context.Notifications.Remove(notification);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification: {Id}", id);
                throw;
            }
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .ToListAsync();

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                    notification.ReadAt = DateTime.UtcNow;
                    notification.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read for user: {UserId}", userId);
                throw;
            }
        }
    }
}
