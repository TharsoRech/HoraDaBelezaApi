using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.GetProfileQuery;

public record GetProfileQuery(int UserId) : IRequest<UserDto>;
