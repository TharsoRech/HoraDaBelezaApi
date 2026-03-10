using HoraDaBeleza.Application.Commands.Professionals;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Queries.ListProfessionalsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

/// <summary>Professionals at a salon</summary>
[Route("api/salons/{salonId:int}/professionals")]
[Tags("Professionals")]
[Produces("application/json")]
public class ProfessionalsController : ApiController
{
    private readonly IMediator _mediator;
    public ProfessionalsController(IMediator mediator) => _mediator = mediator;

    /// <summary>List professionals for a salon (public)</summary>
    /// <param name="salonId">Salon ID</param>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ProfessionalDto>), 200)]
    public async Task<IActionResult> List(int salonId)
        => Ok(await _mediator.Send(new ListProfessionalsQuery(salonId)));

    /// <summary>Link a professional to a salon (owner only)</summary>
    /// <remarks>The user being linked must have Type = **2** (Professional).</remarks>
    /// <response code="201">Professional linked</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ProfessionalDto), 201)]
    public async Task<IActionResult> Create(int salonId, [FromBody] CreateProfessionalRequest request)
    {
        var result = await _mediator.Send(new CreateProfessionalCommand(
            request.UserId, salonId, UserId, request.Specialty, request.Bio));
        return Created("", result);
    }
}
