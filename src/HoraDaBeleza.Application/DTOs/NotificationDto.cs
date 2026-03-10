using HoraDaBeleza.Domain.Enums;

namespace HoraDaBeleza.Application.DTOs;

public record NotificationDto(int Id, string Title, string Message,
    NotificationType Type, bool Read, int? ReferenceId, DateTime CreatedAt);
