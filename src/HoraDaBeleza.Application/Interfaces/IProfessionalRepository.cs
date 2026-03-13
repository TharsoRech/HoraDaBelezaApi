using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Domain.Entities;

namespace HoraDaBeleza.Application.Interfaces;

public interface IProfessionalRepository
{
    Task<Professional?> GetByIdAsync(int id);
    Task<IEnumerable<Professional>> ListBySalonAsync(int salonId);
    Task<int> CreateAsync(Professional professional);
    Task UpdateAsync(Professional professional);
    Task DeleteAsync(int id);
    
    // New methods for mobile app compatibility
    Task<IEnumerable<Professional>> GetTopAsync();
    Task<AvailabilityDto> GetAvailabilityAsync(int professionalId, string date);
    Task<IEnumerable<Salon>> GetSalonsByProfessionalAsync(int professionalId);
}
