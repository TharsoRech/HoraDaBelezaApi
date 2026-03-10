using HoraDaBeleza.Domain.Entities;

namespace HoraDaBeleza.Application.Interfaces;

public interface IPlanRepository
{
    Task<IEnumerable<Plan>> ListActiveAsync();
    Task<Plan?> GetByIdAsync(int id);
}
