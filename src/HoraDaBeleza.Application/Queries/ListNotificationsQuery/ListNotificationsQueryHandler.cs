using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListNotificationsQuery;

public class ListNotificationsQueryHandler : IRequestHandler<ListNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly INotificationRepository _repo;
    public ListNotificationsQueryHandler(INotificationRepository repo) => _repo = repo;

    public async Task<IEnumerable<NotificationDto>> Handle(Queries.ListNotificationsQuery.ListNotificationsQuery req, CancellationToken ct)
    {
        var items = await _repo.ListByUserAsync(req.UserId, req.UnreadOnly);
        return items.Select(n => new NotificationDto(n.Id, n.Title, n.Message, n.Type, n.Read, n.ReferenceId, n.CreatedAt));
    }
}
