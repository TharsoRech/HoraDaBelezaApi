using HoraDaBeleza.Domain.Entities;

namespace HoraDaBeleza.Application.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> ListAsync();
    Task<Category?> GetByIdAsync(int id);
}
