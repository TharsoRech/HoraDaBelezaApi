using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.GetProfessionalAvailabilityQuery;

public record GetProfessionalAvailabilityQuery(int ProfessionalId, string Date) : IRequest<AvailabilityDto>;