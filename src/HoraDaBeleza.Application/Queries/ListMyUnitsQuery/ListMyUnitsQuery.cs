using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListMyUnitsQuery;

public record ListMyUnitsQuery(int UserId) : IRequest<IEnumerable<SalonDto>>;