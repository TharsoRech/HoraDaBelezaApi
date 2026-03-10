using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListCategoriesQuery;

public class ListCategoriesQueryHandler : IRequestHandler<ListCategoriesQuery, IEnumerable<CategoryDto>>
{
    private readonly ICategoryRepository _repo;
    public ListCategoriesQueryHandler(ICategoryRepository repo) => _repo = repo;

    public async Task<IEnumerable<CategoryDto>> Handle(Queries.ListCategoriesQuery.ListCategoriesQuery req, CancellationToken ct)
    {
        var categories = await _repo.ListAsync();
        return categories.Select(c => new CategoryDto(c.Id, c.Name, c.IconUrl));
    }
}
