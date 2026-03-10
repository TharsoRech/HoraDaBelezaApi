using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListNotificationsQuery;

public record ListNotificationsQuery(int UserId, bool UnreadOnly = false) : IRequest<IEnumerable<NotificationDto>>;
