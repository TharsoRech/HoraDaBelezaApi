using HoraDaBeleza.Domain.Enums;

namespace HoraDaBeleza.Application.DTOs;

public record SubscriptionDto(int Id, int SalonId, int PlanId, string PlanName,
    SubscriptionStatus Status, DateTime StartDate, DateTime EndDate);
