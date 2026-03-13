using HoraDaBeleza.Application.Commands.Salons;
using HoraDaBeleza.Application.Commands.Salons.CreateSalonCommand;
using HoraDaBeleza.Application.Commands.Salons.DeleteSalonCommand;
using HoraDaBeleza.Application.Commands.Salons.UpdateSalonCommand;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Queries.GetProfessionalsByIdsQuery;
using HoraDaBeleza.Application.Queries.GetSalonQuery;
using HoraDaBeleza.Application.Queries.GetSalonReviewsQuery;
using HoraDaBeleza.Application.Queries.GetServicesByIdsQuery;
using HoraDaBeleza.Application.Queries.ListMyUnitsQuery;
using HoraDaBeleza.Application.Queries.ListPopularSalonsQuery;
using HoraDaBeleza.Application.Queries.ListSalonsByOwnerQuery;
using HoraDaBeleza.Application.Queries.ListSalonsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

/// <summary>Salon management</summary>
[Route("api/salons")]
[Tags("Salons")]
[Produces("application/json")]
public class SalonsController : ApiController
{
    private readonly IMediator _mediator;
    public SalonsController(IMediator mediator) => _mediator = mediator;

    /// <summary>List all salons (public)</summary>
    /// <param name="city">Filter by city</param>
    /// <param name="search">Search by name or description</param>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<SalonDto>), 200)]
    public async Task<IActionResult> List([FromQuery] string? city, [FromQuery] string? search)
        => Ok(await _mediator.Send(new ListSalonsQuery(city, search)));

    /// <summary>Get salon by ID (public)</summary>
    /// <response code="200">Salon found</response>
    /// <response code="404">Salon not found</response>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SalonDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _mediator.Send(new GetSalonQuery(id)));

    /// <summary>List my salons (owner only)</summary>
    [HttpGet("mine")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<SalonDto>), 200)]
    public async Task<IActionResult> MySalons()
        => Ok(await _mediator.Send(new ListSalonsByOwnerQuery(UserId)));

    /// <summary>Create a salon</summary>
    /// <response code="201">Salon created</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(SalonDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateSalonRequest request)
    {
        var result = await _mediator.Send(new CreateSalonCommand(
            UserId, request.Name, request.Description, request.Address,
            request.City, request.State, request.ZipCode,
            request.Latitude, request.Longitude, request.Phone,
            request.Email, request.BusinessHours));
        return Created($"/api/salons/{result.Id}", result);
    }

    /// <summary>Update a salon (owner only)</summary>
    /// <response code="200">Updated</response>
    /// <response code="403">Not the owner</response>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(SalonDto), 200)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSalonRequest request)
        => Ok(await _mediator.Send(new UpdateSalonCommand(
            id, UserId, request.Name, request.Description, request.Address,
            request.City, request.State, request.ZipCode,
            request.Latitude, request.Longitude, request.Phone,
            request.Email, request.BusinessHours)));

    /// <summary>Deactivate a salon (owner only)</summary>
    /// <response code="204">Deactivated</response>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteSalonCommand(id, UserId));
        return NoContent();
    }

    /// <summary>Get popular salons (public)</summary>
    [HttpGet("popular")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<SalonDto>), 200)]
    public async Task<IActionResult> GetPopular()
        => Ok(await _mediator.Send(new ListPopularSalonsQuery()));

    /// <summary>Get salon services by IDs (public)</summary>
    /// <param name="serviceIds">Comma-separated service IDs</param>
    [HttpGet("services/{serviceIds}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ServiceDto>), 200)]
    public async Task<IActionResult> GetServicesByIds(string serviceIds)
    {
        var ids = serviceIds.Split(',').Select(int.Parse).ToList();
        return Ok(await _mediator.Send(new GetServicesByIdsQuery(ids)));
    }

    /// <summary>Get salon professionals by IDs (public)</summary>
    /// <param name="professionalIds">Comma-separated professional IDs</param>
    [HttpGet("professionals/{professionalIds}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ProfessionalDto>), 200)]
    public async Task<IActionResult> GetProfessionalsByIds(string professionalIds)
    {
        var ids = professionalIds.Split(',').Select(int.Parse).ToList();
        return Ok(await _mediator.Send(new GetProfessionalsByIdsQuery(ids)));
    }

    /// <summary>Get salon reviews (public)</summary>
    /// <param name="id">Salon ID</param>
    [HttpGet("{id:int}/reviews")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ReviewDto>), 200)]
    public async Task<IActionResult> GetReviews(int id)
        => Ok(await _mediator.Send(new GetSalonReviewsQuery(id)));

    /// <summary>Get my units (owner only)</summary>
    [HttpGet("my-units")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<SalonDto>), 200)]
    public async Task<IActionResult> GetMyUnits()
        => Ok(await _mediator.Send(new ListMyUnitsQuery(UserId)));
}
