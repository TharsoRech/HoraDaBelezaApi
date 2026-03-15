namespace HoraDaBeleza.Domain.Entities;

public class Salon
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }  // Base64 image
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string? ZipCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? BusinessHours { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public string? Rating { get; set; } 
    public int Reviews { get; set; } = 0;
    public string? Gallery { get; set; } 
    public bool UserHasVisited { get; set; } 
    public bool Published { get => Active; }
    public bool IsAdmin { get; set; } 
    public decimal? AverageRating { get; set; }
    public string? WhatsApp { get; set; }
}
