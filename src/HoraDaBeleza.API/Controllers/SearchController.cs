using HoraDaBeleza.Application.Queries.SearchQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

/// <summary>Search functionality for salons, services, and professionals</summary>
[Route("api/search")]
[Tags("Search")]
[Produces("application/json")]
public class SearchController : ApiController
{
    private readonly IMediator _mediator;
    public SearchController(IMediator mediator) => _mediator = mediator;

    /// <summary>Search across salons, services, and professionals</summary>
    /// <param name="query">Search query string</param>
    /// <param name="filter">Filter type: 'Salão', 'Serviço', or 'Pessoas'</param>
    /// <param name="page">Page number (starts at 1)</param>
    /// <param name="limit">Number of results per page (default: 5)</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] string filter, [FromQuery] int page = 1, [FromQuery] int limit = 5)
    {
        var result = await _mediator.Send(new SearchQuery(query, filter, page, limit));
        return Ok(result);
    }
}