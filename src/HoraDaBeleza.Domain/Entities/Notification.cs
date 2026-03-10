using HoraDaBeleza.Domain.Enums;

namespace HoraDaBeleza.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool Read { get; set; } = false;
    public int? ReferenceId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
