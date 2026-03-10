using Dapper;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Infrastructure.Data;

namespace HoraDaBeleza.Infrastructure.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly IDbConnectionFactory _db;
    public SubscriptionRepository(IDbConnectionFactory db) => _db = db;

    public async Task<Subscription?> GetActiveBySalonAsync(int salonId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Subscription>(
            "SELECT * FROM Subscriptions WHERE SalonId=@SalonId AND Status=1", new { SalonId = salonId });
    }

    public async Task<int> CreateAsync(Subscription subscription)
    {
        using var conn = _db.CreateConnection();
        const string sql = @"
            INSERT INTO Subscriptions (SalonId,PlanId,Status,StartDate,EndDate,CreatedAt)
            VALUES (@SalonId,@PlanId,@Status,@StartDate,@EndDate,@CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.QuerySingleAsync<int>(sql, subscription);
    }

    public async Task UpdateAsync(Subscription subscription)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("UPDATE Subscriptions SET Status=@Status WHERE Id=@Id", subscription);
    }
}
