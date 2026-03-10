namespace HoraDaBeleza.Application.DTOs;

public record SalonDto(int Id, int OwnerId, string Name, string? Description, string? LogoUrl,
    string Address, string City, string State, string? Phone,
    decimal? Latitude, decimal? Longitude, decimal? AverageRating, bool Active);
