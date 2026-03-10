using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Application.Queries.ListNotificationsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

/// <summary>User notifications</summary>
[Route("api/notifications")]
[Tags("Notifications")]
[Produces("application/json")]
public class NotificationsController : ApiController
{
    private readonly IMediator               _mediator;
    private readonly INotificationRepository _repo;

    public NotificationsController(IMediator mediator, INotificationRepository repo)
    {
        _mediator = mediator;
        _repo     = repo;
    }

    /// <summary>List my notifications</summary>
    /// <param name="unreadOnly">If true, returns only unread notifications</param>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<NotificationDto>), 200)]
    public async Task<IActionResult> List([FromQuery] bool unreadOnly = false)
        => Ok(await _mediator.Send(new ListNotificationsQuery(UserId, unreadOnly)));

    /// <summary>Mark a notification as read</summary>
    /// <response code="204">Marked as read</response>
    [HttpPatch("{id:int}/read")]
    [Authorize]
    [ProducesResponseType(204)]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _repo.MarkAsReadAsync(id, UserId);
        return NoContent();
    }

    /// <summary>Mark all notifications as read</summary>
    /// <response code="204">All marked as read</response>
    [HttpPatch("read-all")]
    [Authorize]
    [ProducesResponseType(204)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        await _repo.MarkAllAsReadAsync(UserId);
        return NoContent();
    }
}
