using Dapper;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Infrastructure.Data;

namespace HoraDaBeleza.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _db;
    public UserRepository(IDbConnectionFactory db) => _db = db;

    public async Task<User?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Id=@Id AND Active=1", new { Id = id });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Email=@Email", new { Email = email.ToLower() });
    }

    public async Task<int> CreateAsync(User user)
    {
        using var conn = _db.CreateConnection();
        const string sql = @"
            INSERT INTO Users (Name,Email,PasswordHash,Phone,Base64Image,Type,Active,CreatedAt)
            VALUES (@Name,@Email,@PasswordHash,@Phone,@Base64Image,@Type,@Active,@CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.QuerySingleAsync<int>(sql, user);
    }

    public async Task UpdateAsync(User user)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE Users SET Name=@Name,Phone=@Phone,Base64Image=@Base64Image,UpdatedAt=@UpdatedAt WHERE Id=@Id",
            user);
    }

    public async Task<bool> EmailExistsAsync(string email, int? ignoreId = null)
    {
        using var conn = _db.CreateConnection();
        var sql = ignoreId.HasValue
            ? "SELECT COUNT(1) FROM Users WHERE Email=@Email AND Id<>@IgnoreId"
            : "SELECT COUNT(1) FROM Users WHERE Email=@Email";
        return await conn.QuerySingleAsync<int>(sql, new { Email = email.ToLower(), IgnoreId = ignoreId }) > 0;
    }
}
