using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Queries.ListCategoriesQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

/// <summary>Service categories</summary>
[Route("api/categories")]
[Tags("Categories")]
[Produces("application/json")]
public class CategoriesController : ApiController
{
    private readonly IMediator _mediator;
    public CategoriesController(IMediator mediator) => _mediator = mediator;

    /// <summary>List all active categories (public)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), 200)]
    public async Task<IActionResult> List()
        => Ok(await _mediator.Send(new ListCategoriesQuery()));
}
