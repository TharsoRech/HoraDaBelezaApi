using HoraDaBeleza.Domain.Enums;

namespace HoraDaBeleza.Application.DTOs;

public record AppointmentDto(int Id, int ClientId, string ClientName, int ProfessionalId,
    string ProfessionalName, int ServiceId, string ServiceName, int SalonId, string SalonName,
    DateTime ScheduledAt, int DurationMinutes, decimal TotalPrice,
    AppointmentStatus Status, string? Notes, DateTime CreatedAt);
