using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Domain.Enums;

namespace HoraDaBeleza.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<int> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> EmailExistsAsync(string email, int? ignoreId = null);
}

public interface ISalonRepository
{
    Task<Salon?> GetByIdAsync(int id);
    Task<IEnumerable<Salon>> ListAsync(string? city = null, string? search = null);
    Task<IEnumerable<Salon>> ListByOwnerAsync(int ownerId);
    Task<int> CreateAsync(Salon salon);
    Task UpdateAsync(Salon salon);
    Task DeleteAsync(int id);
}

public interface IProfessionalRepository
{
    Task<Professional?> GetByIdAsync(int id);
    Task<IEnumerable<Professional>> ListBySalonAsync(int salonId);
    Task<int> CreateAsync(Professional professional);
    Task UpdateAsync(Professional professional);
    Task DeleteAsync(int id);
}

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(int id);
    Task<IEnumerable<Service>> ListBySalonAsync(int salonId, int? categoryId = null);
    Task<int> CreateAsync(Service service);
    Task UpdateAsync(Service service);
    Task DeleteAsync(int id);
}

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(int id);
    Task<IEnumerable<Appointment>> ListByClientAsync(int clientId);
    Task<IEnumerable<Appointment>> ListByProfessionalAsync(int professionalId, DateTime? date = null);
    Task<IEnumerable<Appointment>> ListBySalonAsync(int salonId, DateTime? date = null);
    Task<bool> HasConflictAsync(int professionalId, DateTime scheduledAt, int durationMinutes, int? ignoreId = null);
    Task<int> CreateAsync(Appointment appointment);
    Task UpdateStatusAsync(int id, AppointmentStatus status);
}

public interface IReviewRepository
{
    Task<IEnumerable<Review>> ListBySalonAsync(int salonId);
    Task<IEnumerable<Review>> ListByProfessionalAsync(int professionalId);
    Task<bool> ReviewExistsForAppointmentAsync(int appointmentId);
    Task<int> CreateAsync(Review review);
}

public interface IPlanRepository
{
    Task<IEnumerable<Plan>> ListActiveAsync();
    Task<Plan?> GetByIdAsync(int id);
}

public interface ISubscriptionRepository
{
    Task<Subscription?> GetActiveBySalonAsync(int salonId);
    Task<int> CreateAsync(Subscription subscription);
    Task UpdateAsync(Subscription subscription);
}

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> ListByUserAsync(int userId, bool unreadOnly = false);
    Task<int> CreateAsync(Notification notification);
    Task MarkAsReadAsync(int id, int userId);
    Task MarkAllAsReadAsync(int userId);
}

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> ListAsync();
    Task<Category?> GetByIdAsync(int id);
}

public interface ITokenService
{
    string GenerateToken(User user);
}
