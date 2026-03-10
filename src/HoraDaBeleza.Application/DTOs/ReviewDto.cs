namespace HoraDaBeleza.Application.DTOs;

public record ReviewDto(int Id, int AppointmentId, string ClientName, int Rating,
    string? Comment, DateTime CreatedAt);
