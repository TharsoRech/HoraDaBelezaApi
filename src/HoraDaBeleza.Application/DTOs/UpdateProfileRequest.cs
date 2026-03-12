namespace HoraDaBeleza.Application.DTOs;

public record UpdateProfileRequest(string Name, string? Phone, string? PhotoUrl, string? Base64Image);
