using HoraDaBeleza.Application.Commands.Services;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Queries.ListServicesQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

/// <summary>Services offered by a salon</summary>
[Route("api/salons/{salonId:int}/services")]
[Tags("Services")]
[Produces("application/json")]
public class ServicesController : ApiController
{
    private readonly IMediator _mediator;
    public ServicesController(IMediator mediator) => _mediator = mediator;

    /// <summary>List services for a salon (public)</summary>
    /// <param name="salonId">Salon ID</param>
    /// <param name="categoryId">Filter by category (optional)</param>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ServiceDto>), 200)]
    public async Task<IActionResult> List(int salonId, [FromQuery] int? categoryId)
        => Ok(await _mediator.Send(new ListServicesQuery(salonId, categoryId)));

    /// <summary>Create a service (owner only)</summary>
    /// <response code="201">Service created</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ServiceDto), 201)]
    public async Task<IActionResult> Create(int salonId, [FromBody] CreateServiceRequest request)
    {
        var result = await _mediator.Send(new CreateServiceCommand(
            salonId, UserId, request.CategoryId, request.Name,
            request.Description, request.Price, request.DurationMinutes));
        return Created("", result);
    }

    /// <summary>Update a service (owner only)</summary>
    /// <response code="204">Updated</response>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Update(int salonId, int id, [FromBody] UpdateServiceRequest request)
    {
        await _mediator.Send(new UpdateServiceCommand(
            id, salonId, UserId, request.Name, request.Description,
            request.Price, request.DurationMinutes, request.Active));
        return NoContent();
    }

    /// <summary>Delete a service (owner only)</summary>
    /// <response code="204">Deleted</response>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int salonId, int id)
    {
        await _mediator.Send(new DeleteServiceCommand(id, salonId, UserId));
        return NoContent();
    }
}
