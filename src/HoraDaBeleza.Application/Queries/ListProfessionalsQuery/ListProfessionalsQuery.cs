using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListProfessionalsQuery;

public record ListProfessionalsQuery(int SalonId) : IRequest<IEnumerable<ProfessionalDto>>;
