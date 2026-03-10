using HoraDaBeleza.Domain.Enums;

namespace HoraDaBeleza.Application.DTOs;

public record UserDto(int Id, string Name, string Email, string? Phone, string? PhotoUrl, UserType Type, bool Active);
