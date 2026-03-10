using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Review.CreateReviewCommand;

public class CreateReviewCommandHandler(
    IReviewRepository reviewRepo,
    IAppointmentRepository appointmentRepo,
    IUserRepository userRepo
) : IRequestHandler<CreateReviewCommand, ReviewDto>
{

    public async Task<ReviewDto> Handle(CreateReviewCommand req, CancellationToken ct)
    {
        var appointment = await appointmentRepo.GetByIdAsync(req.AppointmentId)
                          ?? throw new NotFoundException("Appointment", req.AppointmentId);

        if (appointment.ClientId != req.ClientId)
            throw new UnauthorizedException("You cannot review this appointment.");

        if (appointment.Status != Domain.Enums.AppointmentStatus.Completed)
            throw new BusinessException("You can only review completed appointments.");

        if (await reviewRepo.ReviewExistsForAppointmentAsync(req.AppointmentId))
            throw new BusinessException("This appointment has already been reviewed.");

        var client = await userRepo.GetByIdAsync(req.ClientId);

        var review = new Domain.Entities.Review
        {
            AppointmentId  = req.AppointmentId,
            ClientId       = req.ClientId,
            ProfessionalId = appointment.ProfessionalId,
            SalonId        = appointment.SalonId,
            Rating         = req.Rating,
            Comment        = req.Comment
        };

        var id = await reviewRepo.CreateAsync(review);
        return new ReviewDto(id, req.AppointmentId, client?.Name ?? "", req.Rating, req.Comment, DateTime.UtcNow);
    }
}
