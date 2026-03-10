using HoraDaBeleza.Application.Commands.Subscriptions;
using HoraDaBeleza.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

/// <summary>Salon plan subscriptions</summary>
[Route("api/subscriptions")]
[Tags("Plans & Subscriptions")]
[Produces("application/json")]
public class SubscriptionsController : ApiController
{
    private readonly IMediator _mediator;
    public SubscriptionsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Subscribe a salon to a plan (owner only)</summary>
    /// <remarks>If the salon already has an active subscription, it will be cancelled automatically.</remarks>
    /// <response code="201">Subscription created</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(SubscriptionDto), 201)]
    public async Task<IActionResult> Subscribe([FromBody] CreateSubscriptionRequest request)
    {
        var result = await _mediator.Send(
            new CreateSubscriptionCommand(request.SalonId, UserId, request.PlanId));
        return Created("", result);
    }
}
