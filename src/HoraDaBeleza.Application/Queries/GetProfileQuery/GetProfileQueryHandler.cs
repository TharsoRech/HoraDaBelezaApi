using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Queries.GetProfileQuery;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UserDto>
{
    private readonly IUserRepository _repo;
    public GetProfileQueryHandler(IUserRepository repo) => _repo = repo;

    public async Task<UserDto> Handle(GetProfileQuery req, CancellationToken ct)
    {
        var u = await _repo.GetByIdAsync(req.UserId) ?? throw new NotFoundException("User", req.UserId);
        return new UserDto(u.Id, u.Name, u.Email, u.Phone, u.PhotoUrl, u.Type, u.Active);
    }
}
