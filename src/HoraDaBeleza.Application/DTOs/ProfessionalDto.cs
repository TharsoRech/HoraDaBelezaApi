namespace HoraDaBeleza.Application.DTOs;

public record ProfessionalDto(
    int Id, 
    int UserId, 
    int SalonId, 
    string UserName, 
    string? PhotoUrl,
    string? Specialty, 
    string? Bio, 
    decimal? AverageRating, 
    int TotalReviews, 
    bool Active,
    string? AvailableTimes,
    string? Cpf,
    bool IsAdmin);
