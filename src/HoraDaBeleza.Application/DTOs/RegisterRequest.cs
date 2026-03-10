using HoraDaBeleza.Domain.Enums;

namespace HoraDaBeleza.Application.DTOs;

public record RegisterRequest(string Name, string Email, string Password, string? Phone, UserType Type);
