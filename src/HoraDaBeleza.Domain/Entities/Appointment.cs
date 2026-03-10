using HoraDaBeleza.Domain.Enums;

namespace HoraDaBeleza.Domain.Entities;

public class Appointment
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int ProfessionalId { get; set; }
    public int ServiceId { get; set; }
    public int SalonId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public decimal TotalPrice { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
