using HoraDaBeleza.Application.Commands.Appointments;
using HoraDaBeleza.Application.Commands.Appointments.CancelAppointmentCommand;
using HoraDaBeleza.Application.Commands.Appointments.UpdateAppointmentStatusCommand;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Queries.ListClientAppointmentsQuery;
using HoraDaBeleza.Application.Queries.ListProfessionalAppointmentsQuery;
using HoraDaBeleza.Application.Queries.ListSalonAppointmentsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

/// <summary>Appointment scheduling</summary>
[Route("api/appointments")]
[Tags("Appointments")]
[Produces("application/json")]
public class AppointmentsController : ApiController
{
    private readonly IMediator _mediator;
    public AppointmentsController(IMediator mediator) => _mediator = mediator;

    /// <summary>My appointments (authenticated client)</summary>
    [HttpGet("mine")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<AppointmentDto>), 200)]
    public async Task<IActionResult> Mine()
        => Ok(await _mediator.Send(new ListClientAppointmentsQuery(UserId)));

    /// <summary>Appointments for a salon</summary>
    /// <param name="salonId">Salon ID</param>
    /// <param name="date">Filter by date (optional)</param>
    [HttpGet("salon/{salonId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<AppointmentDto>), 200)]
    public async Task<IActionResult> BySalon(int salonId, [FromQuery] DateTime? date)
        => Ok(await _mediator.Send(new ListSalonAppointmentsQuery(salonId, date)));

    /// <summary>Appointments for a professional</summary>
    /// <param name="professionalId">Professional ID</param>
    /// <param name="date">Filter by date (optional)</param>
    [HttpGet("professional/{professionalId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<AppointmentDto>), 200)]
    public async Task<IActionResult> ByProfessional(int professionalId, [FromQuery] DateTime? date)
        => Ok(await _mediator.Send(new ListProfessionalAppointmentsQuery(professionalId, date)));

    /// <summary>Create an appointment</summary>
    /// <remarks>
    /// Duration and price are inherited from the selected service.
    /// Time conflicts are automatically validated.
    /// </remarks>
    /// <response code="201">Appointment created</response>
    /// <response code="422">Time slot unavailable or service inactive</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(AppointmentDto), 201)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
    {
        var result = await _mediator.Send(new CreateAppointmentCommand(
            UserId, request.ProfessionalId, request.ServiceId,
            request.SalonId, request.ScheduledAt, request.Notes));
        return Created($"/api/appointments/{result.Id}", result);
    }

    /// <summary>Cancel an appointment</summary>
    /// <remarks>Can be cancelled by the client or the professional.</remarks>
    /// <response code="204">Cancelled</response>
    /// <response code="422">Already completed or already cancelled</response>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Cancel(int id)
    {
        await _mediator.Send(new CancelAppointmentCommand(id, UserId));
        return NoContent();
    }

    /// <summary>Update appointment status (professional)</summary>
    /// <remarks>Statuses: **1**=Pending **2**=Confirmed **3**=Cancelled **4**=Completed **5**=NoShow</remarks>
    /// <response code="204">Status updated</response>
    [HttpPatch("{id:int}/status")]
    [Authorize]
    [ProducesResponseType(204)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAppointmentStatusRequest request)
    {
        await _mediator.Send(new UpdateAppointmentStatusCommand(id, UserId, request.Status));
        return NoContent();
    }
}
