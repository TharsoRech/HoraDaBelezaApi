using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListSalonsQuery;

public record ListSalonsQuery(string? City, string? Search) : IRequest<IEnumerable<SalonDto>>;
