namespace HoraDaBeleza.Application.DTOs;

public record PlanDto(int Id, string Name, string? Description, decimal Price,
    int PeriodDays, int AppointmentLimit);
