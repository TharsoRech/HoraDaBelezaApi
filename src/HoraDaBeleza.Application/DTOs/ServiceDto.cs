namespace HoraDaBeleza.Application.DTOs;

public record ServiceDto(int Id, int SalonId, int CategoryId, string CategoryName, string Name,
    string? Description, decimal Price, int DurationMinutes, bool Active);
