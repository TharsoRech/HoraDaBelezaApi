using HoraDaBeleza.Application.Commands.Professional.CreateProfessionalCommand;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Queries.GetProfessionalAvailabilityQuery;
using HoraDaBeleza.Application.Queries.GetSalonsByProfessionalQuery;
using HoraDaBeleza.Application.Queries.GetTopProfessionalsQuery;
using HoraDaBeleza.Application.Queries.ListProfessionalsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

/// <summary>Professionals at a salon</summary>
[Route("api/salons/{salonId:int}/professionals")]
[Tags("Professionals")]
[Produces("application/json")]
public class ProfessionalsController(IMediator mediator) : ApiController
{

    /// <summary>List professionals for a salon (public)</summary>
    /// <param name="salonId">Salon ID</param>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ProfessionalDto>), 200)]
    public async Task<IActionResult> List(int salonId)
        => Ok(await mediator.Send(new ListProfessionalsQuery(salonId)));

    /// <summary>Link a professional to a salon (owner only)</summary>
    /// <remarks>The user being linked must have Type = **2** (Professional).</remarks>
    /// <response code="201">Professional linked</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ProfessionalDto), 201)]
    public async Task<IActionResult> Create(int salonId, [FromBody] CreateProfessionalRequest request)
    {
        var result = await mediator.Send(new CreateProfessionalCommand(
            request.UserId, salonId, UserId, request.Specialty, request.Bio));
        return Created("", result);
    }

    /// <summary>Get top professionals (public)</summary>
    [HttpGet("top")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ProfessionalDto>), 200)]
    public async Task<IActionResult> GetTop()
        => Ok(await mediator.Send(new GetTopProfessionalsQuery()));

    /// <summary>Get professional availability (public)</summary>
                                                                /// <param name="professionalId">Professional ID</param>
    /// <param name="date">Date to check availability</param>
    [HttpGet("{professionalId:int}/availability")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AvailabilityDto), 200)]
    public async Task<IActionResult> GetAvailability(int professionalId, [FromQuery] string date)
    {
        var result = await mediator.Send(new GetProfessionalAvailabilityQuery(professionalId, date));
        return Ok(result);
    }

    /// <summary>Get salons by professional (public)</summary>
    /// <param name="professionalId">Professional ID</param>
    [HttpGet("{professionalId:int}/salons")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<SalonDto>), 200)]
    public async Task<IActionResult> GetSalonsByProfessional(int professionalId)
        => Ok(await mediator.Send(new GetSalonsByProfessionalQuery(professionalId)));
}
