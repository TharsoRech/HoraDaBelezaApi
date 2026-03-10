using HoraDaBeleza.Domain.Entities;

namespace HoraDaBeleza.Application.Interfaces;

public interface IProfessionalRepository
{
    Task<Professional?> GetByIdAsync(int id);
    Task<IEnumerable<Professional>> ListBySalonAsync(int salonId);
    Task<int> CreateAsync(Professional professional);
    Task UpdateAsync(Professional professional);
    Task DeleteAsync(int id);
}
