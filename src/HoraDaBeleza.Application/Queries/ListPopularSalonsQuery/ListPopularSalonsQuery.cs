using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListPopularSalonsQuery;

public record ListPopularSalonsQuery : IRequest<IEnumerable<SalonDto>>;