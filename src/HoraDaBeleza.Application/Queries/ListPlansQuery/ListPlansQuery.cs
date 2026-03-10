using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListPlansQuery;

public record ListPlansQuery : IRequest<IEnumerable<PlanDto>>;
