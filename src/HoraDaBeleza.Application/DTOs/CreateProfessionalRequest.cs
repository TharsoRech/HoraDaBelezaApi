namespace HoraDaBeleza.Application.DTOs;

public record CreateProfessionalRequest(int UserId, int SalonId, string? Specialty, string? Bio);
