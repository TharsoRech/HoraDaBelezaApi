namespace HoraDaBeleza.Domain.Entities;

public class Plan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int PeriodDays { get; set; }
    public int AppointmentLimit { get; set; }
    public bool Active { get; set; } = true;
}
