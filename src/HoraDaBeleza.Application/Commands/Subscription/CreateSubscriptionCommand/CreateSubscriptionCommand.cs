using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Subscription.CreateSubscriptionCommand;

public record CreateSubscriptionCommand(int SalonId, int OwnerId, int PlanId) : IRequest<SubscriptionDto>;
