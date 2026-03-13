using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.GetServicesByIdsQuery;

public record GetServicesByIdsQuery(List<int> ServiceIds) : IRequest<IEnumerable<ServiceDto>>;