using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListServicesQuery;

public record ListServicesQuery(int SalonId, int? CategoryId = null) : IRequest<IEnumerable<ServiceDto>>;
