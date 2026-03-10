using HoraDaBeleza.Domain.Enums;

namespace HoraDaBeleza.Application.DTOs;

// ── Auth ───────────────────────────────────────────────────────────────────
public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, string Name, string Email, UserType Type);
public record RegisterRequest(string Name, string Email, string Password, string? Phone, UserType Type);

// ── User ───────────────────────────────────────────────────────────────────
public record UserDto(int Id, string Name, string Email, string? Phone, string? PhotoUrl, UserType Type, bool Active);
public record UpdateProfileRequest(string Name, string? Phone, string? PhotoUrl);

// ── Salon ──────────────────────────────────────────────────────────────────
public record SalonDto(int Id, int OwnerId, string Name, string? Description, string? LogoUrl,
    string Address, string City, string State, string? Phone,
    decimal? Latitude, decimal? Longitude, decimal? AverageRating, bool Active);

public record CreateSalonRequest(string Name, string? Description, string Address, string City,
    string State, string? ZipCode, decimal? Latitude, decimal? Longitude,
    string? Phone, string? Email, string? BusinessHours);

public record UpdateSalonRequest(string Name, string? Description, string Address, string City,
    string State, string? ZipCode, decimal? Latitude, decimal? Longitude,
    string? Phone, string? Email, string? BusinessHours);

// ── Professional ───────────────────────────────────────────────────────────
public record ProfessionalDto(int Id, int UserId, int SalonId, string UserName, string? PhotoUrl,
    string? Specialty, string? Bio, decimal? AverageRating, int TotalReviews, bool Active);

public record CreateProfessionalRequest(int UserId, int SalonId, string? Specialty, string? Bio);

// ── Service ────────────────────────────────────────────────────────────────
public record ServiceDto(int Id, int SalonId, int CategoryId, string CategoryName, string Name,
    string? Description, decimal Price, int DurationMinutes, bool Active);

public record CreateServiceRequest(int CategoryId, string Name, string? Description, decimal Price, int DurationMinutes);
public record UpdateServiceRequest(string Name, string? Description, decimal Price, int DurationMinutes, bool Active);

// ── Category ───────────────────────────────────────────────────────────────
public record CategoryDto(int Id, string Name, string? IconUrl);

// ── Appointment ────────────────────────────────────────────────────────────
public record AppointmentDto(int Id, int ClientId, string ClientName, int ProfessionalId,
    string ProfessionalName, int ServiceId, string ServiceName, int SalonId, string SalonName,
    DateTime ScheduledAt, int DurationMinutes, decimal TotalPrice,
    AppointmentStatus Status, string? Notes, DateTime CreatedAt);

public record CreateAppointmentRequest(int ProfessionalId, int ServiceId, int SalonId,
    DateTime ScheduledAt, string? Notes);

public record UpdateAppointmentStatusRequest(AppointmentStatus Status);

// ── Review ─────────────────────────────────────────────────────────────────
public record ReviewDto(int Id, int AppointmentId, string ClientName, int Rating,
    string? Comment, DateTime CreatedAt);

public record CreateReviewRequest(int AppointmentId, int Rating, string? Comment);

// ── Plan ───────────────────────────────────────────────────────────────────
public record PlanDto(int Id, string Name, string? Description, decimal Price,
    int PeriodDays, int AppointmentLimit);

// ── Subscription ───────────────────────────────────────────────────────────
public record SubscriptionDto(int Id, int SalonId, int PlanId, string PlanName,
    SubscriptionStatus Status, DateTime StartDate, DateTime EndDate);

public record CreateSubscriptionRequest(int SalonId, int PlanId);

// ── Notification ───────────────────────────────────────────────────────────
public record NotificationDto(int Id, string Title, string Message,
    NotificationType Type, bool Read, int? ReferenceId, DateTime CreatedAt);
