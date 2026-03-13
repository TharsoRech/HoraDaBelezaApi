namespace HoraDaBeleza.Application.DTOs;

public record AvailabilityDto(
    int ProfessionalId,
    string Date,
    List<string> AvailableTimes);
