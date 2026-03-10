namespace HoraDaBeleza.Domain.Entities;

public class Service
{
    public int Id { get; set; }
    public int SalonId { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
