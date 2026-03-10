using HoraDaBeleza.Domain.Entities;

namespace HoraDaBeleza.Application.Interfaces;

public interface ISalonRepository
{
    Task<Salon?> GetByIdAsync(int id);
    Task<IEnumerable<Salon>> ListAsync(string? city = null, string? search = null);
    Task<IEnumerable<Salon>> ListByOwnerAsync(int ownerId);
    Task<int> CreateAsync(Salon salon);
    Task UpdateAsync(Salon salon);
    Task DeleteAsync(int id);
}
