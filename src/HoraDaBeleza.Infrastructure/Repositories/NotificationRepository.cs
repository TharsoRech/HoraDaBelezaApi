using Dapper;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Infrastructure.Data;

namespace HoraDaBeleza.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly IDbConnectionFactory _db;
    public NotificationRepository(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<Notification>> ListByUserAsync(int userId, bool unreadOnly = false)
    {
        using var conn = _db.CreateConnection();
        var sql = "SELECT * FROM Notifications WHERE UserId=@UserId";
        if (unreadOnly) sql += " AND [Read]=0";
        sql += " ORDER BY CreatedAt DESC";
        return await conn.QueryAsync<Notification>(sql, new { UserId = userId });
    }

    public async Task<int> CreateAsync(Notification notification)
    {
        using var conn = _db.CreateConnection();
        const string sql = @"
            INSERT INTO Notifications (UserId,Title,Message,Type,[Read],ReferenceId,CreatedAt)
            VALUES (@UserId,@Title,@Message,@Type,@Read,@ReferenceId,@CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.QuerySingleAsync<int>(sql, notification);
    }

    public async Task MarkAsReadAsync(int id, int userId)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE Notifications SET [Read]=1 WHERE Id=@Id AND UserId=@UserId",
            new { Id = id, UserId = userId });
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("UPDATE Notifications SET [Read]=1 WHERE UserId=@UserId", new { UserId = userId });
    }
}
