using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Users.UpdateProfileCommand;

public class UpdateProfileCommandHandler(IUserRepository repo) : IRequestHandler<UpdateProfileCommand, UserDto>
{

    public async Task<UserDto> Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        var user = await repo.GetByIdAsync(request.UserId)
            ?? throw new NotFoundException("User", request.UserId);

        user.Name        = request.Name;
        user.Phone       = request.Phone;
        user.Base64Image = request.Base64Image;
        user.UpdatedAt   = DateTime.UtcNow;

        await repo.UpdateAsync(user);
        return new UserDto(user.Id, user.Name, user.Email, user.Phone, user.Base64Image, user.Type, user.Active, user.Doc, user.Dob?.ToString("yyyy-MM-dd"));
    }
}
