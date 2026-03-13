using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.GetSalonsByProfessionalQuery;

public record GetSalonsByProfessionalQuery(int ProfessionalId) : IRequest<IEnumerable<SalonDto>>;