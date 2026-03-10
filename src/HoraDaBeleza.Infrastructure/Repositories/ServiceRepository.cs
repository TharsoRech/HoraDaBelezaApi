using Dapper;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Infrastructure.Data;

namespace HoraDaBeleza.Infrastructure.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly IDbConnectionFactory _db;
    public ServiceRepository(IDbConnectionFactory db) => _db = db;

    public async Task<Service?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Service>(
            "SELECT * FROM Services WHERE Id=@Id AND Active=1", new { Id = id });
    }

    public async Task<IEnumerable<Service>> ListBySalonAsync(int salonId, int? categoryId = null)
    {
        using var conn = _db.CreateConnection();
        var sql = "SELECT s.* FROM Services s INNER JOIN Categories c ON c.Id=s.CategoryId WHERE s.SalonId=@SalonId AND s.Active=1";
        if (categoryId.HasValue) sql += " AND s.CategoryId=@CategoryId";
        sql += " ORDER BY s.Name";
        return await conn.QueryAsync<Service>(sql, new { SalonId = salonId, CategoryId = categoryId });
    }

    public async Task<int> CreateAsync(Service service)
    {
        using var conn = _db.CreateConnection();
        const string sql = @"
            INSERT INTO Services (SalonId,CategoryId,Name,Description,Price,DurationMinutes,Active,CreatedAt)
            VALUES (@SalonId,@CategoryId,@Name,@Description,@Price,@DurationMinutes,@Active,@CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.QuerySingleAsync<int>(sql, service);
    }

    public async Task UpdateAsync(Service service)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE Services SET Name=@Name,Description=@Description,Price=@Price,DurationMinutes=@DurationMinutes,Active=@Active WHERE Id=@Id",
            service);
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("UPDATE Services SET Active=0 WHERE Id=@Id", new { Id = id });
    }
}
