using HoraDaBeleza.Application.Commands.Reviews;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Queries.ListSalonReviewsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

/// <summary>Ratings and reviews</summary>
[Route("api/reviews")]
[Tags("Reviews")]
[Produces("application/json")]
public class ReviewsController : ApiController
{
    private readonly IMediator _mediator;
    public ReviewsController(IMediator mediator) => _mediator = mediator;

    /// <summary>List reviews for a salon (public)</summary>
    /// <param name="salonId">Salon ID</param>
    [HttpGet("salon/{salonId:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ReviewDto>), 200)]
    public async Task<IActionResult> BySalon(int salonId)
        => Ok(await _mediator.Send(new ListSalonReviewsQuery(salonId)));

    /// <summary>Submit a review for a completed appointment</summary>
    /// <remarks>
    /// - Only the appointment's client can review.
    /// - Appointment must have status **Completed** (4).
    /// - Each appointment can only be reviewed once.
    /// - Professional average rating is recalculated automatically.
    /// </remarks>
    /// <response code="201">Review submitted</response>
    /// <response code="422">Appointment not completed or already reviewed</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ReviewDto), 201)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
    {
        var result = await _mediator.Send(
            new CreateReviewCommand(request.AppointmentId, UserId, request.Rating, request.Comment));
        return Created("", result);
    }
}
