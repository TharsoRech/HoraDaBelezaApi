using HoraDaBeleza.Domain.Enums;

namespace HoraDaBeleza.Domain.Entities;

public class Subscription
{
    public int Id { get; set; }
    public int SalonId { get; set; }
    public int PlanId { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
