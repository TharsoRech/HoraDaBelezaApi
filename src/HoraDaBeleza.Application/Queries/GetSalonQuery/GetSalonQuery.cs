using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.GetSalonQuery;

public record GetSalonQuery(int Id) : IRequest<SalonDto>;
