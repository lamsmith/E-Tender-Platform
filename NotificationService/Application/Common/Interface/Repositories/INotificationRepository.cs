using NotificationService.Domain.Entities;
using NotificationService.Domain.Paging;

namespace NotificationService.Application.Common.Interface.Repositories
{
    public interface INotificationRepository
    {
        Task<Notification> GetByIdAsync(Guid id);
        Task<PaginatedList<Notification>> GetUserNotificationsAsync(Guid userId, PageRequest pageRequest);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task DeleteAsync(Guid id);
        Task MarkAllAsReadAsync(Guid userId);
    }
}
