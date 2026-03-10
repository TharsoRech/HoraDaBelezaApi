using Dapper;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Infrastructure.Data;

namespace HoraDaBeleza.Infrastructure.Repositories;

public class SalonRepository : ISalonRepository
{
    private readonly IDbConnectionFactory _db;
    public SalonRepository(IDbConnectionFactory db) => _db = db;

    public async Task<Salon?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Salon>(
            "SELECT * FROM Salons WHERE Id=@Id AND Active=1", new { Id = id });
    }

    public async Task<IEnumerable<Salon>> ListAsync(string? city = null, string? search = null)
    {
        using var conn = _db.CreateConnection();
        var sql = "SELECT * FROM Salons WHERE Active=1";
        if (!string.IsNullOrWhiteSpace(city))   sql += " AND City=@City";
        if (!string.IsNullOrWhiteSpace(search)) sql += " AND (Name LIKE @Search OR Description LIKE @Search)";
        sql += " ORDER BY Name";
        return await conn.QueryAsync<Salon>(sql, new { City = city, Search = $"%{search}%" });
    }

    public async Task<IEnumerable<Salon>> ListByOwnerAsync(int ownerId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Salon>(
            "SELECT * FROM Salons WHERE OwnerId=@OwnerId AND Active=1 ORDER BY Name",
            new { OwnerId = ownerId });
    }

    public async Task<int> CreateAsync(Salon salon)
    {
        using var conn = _db.CreateConnection();
        const string sql = @"
            INSERT INTO Salons (OwnerId,Name,Description,LogoUrl,Address,City,State,ZipCode,
                Latitude,Longitude,Phone,Email,BusinessHours,Active,CreatedAt)
            VALUES (@OwnerId,@Name,@Description,@LogoUrl,@Address,@City,@State,@ZipCode,
                @Latitude,@Longitude,@Phone,@Email,@BusinessHours,@Active,@CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.QuerySingleAsync<int>(sql, salon);
    }

    public async Task UpdateAsync(Salon salon)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(@"
            UPDATE Salons SET Name=@Name,Description=@Description,Address=@Address,City=@City,
                State=@State,ZipCode=@ZipCode,Latitude=@Latitude,Longitude=@Longitude,
                Phone=@Phone,Email=@Email,BusinessHours=@BusinessHours,UpdatedAt=@UpdatedAt
            WHERE Id=@Id", salon);
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("UPDATE Salons SET Active=0 WHERE Id=@Id", new { Id = id });
    }
}
