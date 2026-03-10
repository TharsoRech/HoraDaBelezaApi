using Dapper;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Domain.Enums;
using HoraDaBeleza.Infrastructure.Data;

namespace HoraDaBeleza.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly IDbConnectionFactory _db;
    public AppointmentRepository(IDbConnectionFactory db) => _db = db;

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Appointment>(
            "SELECT * FROM Appointments WHERE Id=@Id", new { Id = id });
    }

    public async Task<IEnumerable<Appointment>> ListByClientAsync(int clientId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Appointment>(
            "SELECT * FROM Appointments WHERE ClientId=@ClientId ORDER BY ScheduledAt DESC",
            new { ClientId = clientId });
    }

    public async Task<IEnumerable<Appointment>> ListByProfessionalAsync(int professionalId, DateTime? date = null)
    {
        using var conn = _db.CreateConnection();
        var sql = "SELECT * FROM Appointments WHERE ProfessionalId=@ProfessionalId";
        if (date.HasValue) sql += " AND CAST(ScheduledAt AS DATE)=@Date";
        sql += " ORDER BY ScheduledAt";
        return await conn.QueryAsync<Appointment>(sql, new { ProfessionalId = professionalId, Date = date?.Date });
    }

    public async Task<IEnumerable<Appointment>> ListBySalonAsync(int salonId, DateTime? date = null)
    {
        using var conn = _db.CreateConnection();
        var sql = "SELECT * FROM Appointments WHERE SalonId=@SalonId AND Status NOT IN (3)";
        if (date.HasValue) sql += " AND CAST(ScheduledAt AS DATE)=@Date";
        sql += " ORDER BY ScheduledAt";
        return await conn.QueryAsync<Appointment>(sql, new { SalonId = salonId, Date = date?.Date });
    }

    public async Task<bool> HasConflictAsync(int professionalId, DateTime scheduledAt, int durationMinutes, int? ignoreId = null)
    {
        using var conn = _db.CreateConnection();
        var end  = scheduledAt.AddMinutes(durationMinutes);
        var sql  = @"SELECT COUNT(1) FROM Appointments
                    WHERE ProfessionalId=@ProfessionalId AND Status NOT IN (3,5)
                    AND (
                        (@Start >= ScheduledAt AND @Start < DATEADD(MINUTE,DurationMinutes,ScheduledAt))
                        OR (@End > ScheduledAt AND @End <= DATEADD(MINUTE,DurationMinutes,ScheduledAt))
                        OR (ScheduledAt >= @Start AND ScheduledAt < @End)
                    )";
        if (ignoreId.HasValue) sql += " AND Id<>@IgnoreId";
        return await conn.QuerySingleAsync<int>(sql,
            new { ProfessionalId = professionalId, Start = scheduledAt, End = end, IgnoreId = ignoreId }) > 0;
    }

    public async Task<int> CreateAsync(Appointment appointment)
    {
        using var conn = _db.CreateConnection();
        const string sql = @"
            INSERT INTO Appointments (ClientId,ProfessionalId,ServiceId,SalonId,ScheduledAt,
                DurationMinutes,TotalPrice,Status,Notes,CreatedAt)
            VALUES (@ClientId,@ProfessionalId,@ServiceId,@SalonId,@ScheduledAt,
                @DurationMinutes,@TotalPrice,@Status,@Notes,@CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.QuerySingleAsync<int>(sql, appointment);
    }

    public async Task UpdateStatusAsync(int id, AppointmentStatus status)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE Appointments SET Status=@Status,UpdatedAt=@UpdatedAt WHERE Id=@Id",
            new { Id = id, Status = (int)status, UpdatedAt = DateTime.UtcNow });
    }
}
