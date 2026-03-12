using HoraDaBeleza.Domain.Enums;

namespace HoraDaBeleza.Application.DTOs;

public record UserDto(int Id, string Name, string Email, string? Phone, string? Base64Image, UserType Type, bool Active, string? Doc, string? Dob);
