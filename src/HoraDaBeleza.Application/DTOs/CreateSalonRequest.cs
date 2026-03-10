namespace HoraDaBeleza.Application.DTOs;

public record CreateSalonRequest(string Name, string? Description, string Address, string City,
    string State, string? ZipCode, decimal? Latitude, decimal? Longitude,
    string? Phone, string? Email, string? BusinessHours);
