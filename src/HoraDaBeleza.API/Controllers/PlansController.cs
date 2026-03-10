using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Queries.ListPlansQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

/// <summary>Subscription plans</summary>
[Route("api/plans")]
[Tags("Plans & Subscriptions")]
[Produces("application/json")]
public class PlansController : ApiController
{
    private readonly IMediator _mediator;
    public PlansController(IMediator mediator) => _mediator = mediator;

    /// <summary>List all available plans (public)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PlanDto>), 200)]
    public async Task<IActionResult> List()
        => Ok(await _mediator.Send(new ListPlansQuery()));
}
