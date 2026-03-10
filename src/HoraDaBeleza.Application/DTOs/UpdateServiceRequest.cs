namespace HoraDaBeleza.Application.DTOs;

public record UpdateServiceRequest(string Name, string? Description, decimal Price, int DurationMinutes, bool Active);
