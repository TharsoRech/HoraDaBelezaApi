namespace HoraDaBeleza.Application.DTOs;

public record CreateAppointmentRequest(int ProfessionalId, int ServiceId, int SalonId,
    DateTime ScheduledAt, string? Notes);
