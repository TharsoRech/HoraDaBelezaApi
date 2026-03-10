namespace HoraDaBeleza.Application.DTOs;

public record CreateServiceRequest(int CategoryId, string Name, string? Description, decimal Price, int DurationMinutes);
