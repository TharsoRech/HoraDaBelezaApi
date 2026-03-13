using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.GetProfessionalsByIdsQuery;

public record GetProfessionalsByIdsQuery(List<int> ProfessionalIds) : IRequest<IEnumerable<ProfessionalDto>>;