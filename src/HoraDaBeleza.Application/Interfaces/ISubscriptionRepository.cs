using HoraDaBeleza.Domain.Entities;

namespace HoraDaBeleza.Application.Interfaces;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetActiveBySalonAsync(int salonId);
    Task<int> CreateAsync(Subscription subscription);
    Task UpdateAsync(Subscription subscription);
}
