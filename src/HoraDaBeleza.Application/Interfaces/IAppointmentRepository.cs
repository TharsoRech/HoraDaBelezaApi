using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Domain.Enums;

namespace HoraDaBeleza.Application.Interfaces;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(int id);
    Task<IEnumerable<Appointment>> ListByClientAsync(int clientId);
    Task<IEnumerable<Appointment>> ListByProfessionalAsync(int professionalId, DateTime? date = null);
    Task<IEnumerable<Appointment>> ListBySalonAsync(int salonId, DateTime? date = null);
    Task<bool> HasConflictAsync(int professionalId, DateTime scheduledAt, int durationMinutes, int? ignoreId = null);
    Task<int> CreateAsync(Appointment appointment);
    Task UpdateStatusAsync(int id, AppointmentStatus status);
    Task<IEnumerable<Appointment>> GetByUserIdAsync(int userId);
}
