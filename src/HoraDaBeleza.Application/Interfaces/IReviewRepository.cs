using HoraDaBeleza.Domain.Entities;

namespace HoraDaBeleza.Application.Interfaces;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> ListBySalonAsync(int salonId);
    Task<IEnumerable<Review>> ListByProfessionalAsync(int professionalId);
    Task<bool> ReviewExistsForAppointmentAsync(int appointmentId);
    Task<int> CreateAsync(Review review);
}
