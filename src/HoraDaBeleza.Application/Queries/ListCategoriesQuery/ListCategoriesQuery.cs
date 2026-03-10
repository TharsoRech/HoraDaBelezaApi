using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListCategoriesQuery;

public record ListCategoriesQuery : IRequest<IEnumerable<CategoryDto>>;
