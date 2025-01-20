using NotificationService.Domain.Entities;

namespace NotificationService.Application.Common.Interface.Repositories
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<Notification> GetByIdAsync(Guid id);
        Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId);
        Task UpdateAsync(Notification notification);
    }
}
