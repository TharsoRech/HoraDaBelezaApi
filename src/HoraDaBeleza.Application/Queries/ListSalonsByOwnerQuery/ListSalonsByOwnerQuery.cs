using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListSalonsByOwnerQuery;

public record ListSalonsByOwnerQuery(int OwnerId) : IRequest<IEnumerable<SalonDto>>;
