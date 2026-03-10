using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Subscription.CreateSubscriptionCommand;

public class CreateSubscriptionCommandHandler(
    ISubscriptionRepository repo,
    ISalonRepository salonRepo,
    IPlanRepository planRepo
) : IRequestHandler<CreateSubscriptionCommand, SubscriptionDto>
{

    public async Task<SubscriptionDto> Handle(CreateSubscriptionCommand req, CancellationToken ct)
    {
        var salon = await salonRepo.GetByIdAsync(req.SalonId) ?? throw new NotFoundException("Salon", req.SalonId);
        if (salon.OwnerId != req.OwnerId) throw new UnauthorizedException();

        var plan = await planRepo.GetByIdAsync(req.PlanId) ?? throw new NotFoundException("Plan", req.PlanId);

        var active = await repo.GetActiveBySalonAsync(req.SalonId);
        if (active != null)
        {
            active.Status = Domain.Enums.SubscriptionStatus.Cancelled;
            await repo.UpdateAsync(active);
        }

        var now = DateTime.UtcNow;
        var subscription = new Domain.Entities.Subscription
        {
            SalonId   = req.SalonId,
            PlanId    = req.PlanId,
            Status    = Domain.Enums.SubscriptionStatus.Active,
            StartDate = now,
            EndDate   = now.AddDays(plan.PeriodDays)
        };

        var id = await repo.CreateAsync(subscription);
        return new SubscriptionDto(id, req.SalonId, req.PlanId, plan.Name,
            Domain.Enums.SubscriptionStatus.Active, subscription.StartDate, subscription.EndDate);
    }
}
