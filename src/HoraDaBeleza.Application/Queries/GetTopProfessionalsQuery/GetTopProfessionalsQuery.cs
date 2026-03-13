using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.GetTopProfessionalsQuery;

public record GetTopProfessionalsQuery : IRequest<IEnumerable<ProfessionalDto>>;