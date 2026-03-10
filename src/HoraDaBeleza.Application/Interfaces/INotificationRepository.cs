using HoraDaBeleza.Domain.Entities;

namespace HoraDaBeleza.Application.Interfaces;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> ListByUserAsync(int userId, bool unreadOnly = false);
    Task<int> CreateAsync(Notification notification);
    Task MarkAsReadAsync(int id, int userId);
    Task MarkAllAsReadAsync(int userId);
}
