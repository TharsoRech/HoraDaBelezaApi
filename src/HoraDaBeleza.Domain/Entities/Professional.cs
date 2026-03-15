namespace HoraDaBeleza.Domain.Entities;

public class Professional
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SalonId { get; set; }
    public string? Specialty { get; set; }
    public string? Bio { get; set; }
    public decimal? AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Campos adicionais do banco de dados
    public string? AvailableTimes { get; set; }
    public bool IsAdmin { get; set; } = false;
}
