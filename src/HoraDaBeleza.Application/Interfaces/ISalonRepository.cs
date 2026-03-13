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
    
    // New methods for mobile app compatibility
    Task<IEnumerable<Salon>> GetPopularAsync();
    Task<IEnumerable<Service>> GetServicesByIdsAsync(List<int> serviceIds);
    Task<IEnumerable<Professional>> GetProfessionalsByIdsAsync(List<int> professionalIds);
    Task<IEnumerable<Review>> GetReviewsAsync(int salonId);
    Task<IEnumerable<Salon>> GetMyUnitsAsync(int userId);
}
