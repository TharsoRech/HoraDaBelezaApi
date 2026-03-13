namespace HoraDaBeleza.Application.DTOs;

public record SubServiceDto(
    int Id,
    int ServiceId,
    string Name,
    decimal Price,
    string Duration,
    string? Description,
    bool Active);