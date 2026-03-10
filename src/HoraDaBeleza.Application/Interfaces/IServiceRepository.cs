using HoraDaBeleza.Domain.Entities;

namespace HoraDaBeleza.Application.Interfaces;

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(int id);
    Task<IEnumerable<Service>> ListBySalonAsync(int salonId, int? categoryId = null);
    Task<int> CreateAsync(Service service);
    Task UpdateAsync(Service service);
    Task DeleteAsync(int id);
}
