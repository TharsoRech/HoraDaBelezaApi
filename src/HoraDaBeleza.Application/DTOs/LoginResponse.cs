using HoraDaBeleza.Domain.Enums;

namespace HoraDaBeleza.Application.DTOs;

public record LoginResponse(string Token, string Name, string Email, UserType Type, string? Doc = null, string? Dob = null, string? Base64Image = null);
