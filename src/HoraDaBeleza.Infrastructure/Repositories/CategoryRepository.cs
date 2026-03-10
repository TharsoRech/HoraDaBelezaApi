using Dapper;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Infrastructure.Data;

namespace HoraDaBeleza.Infrastructure.Repositories;
public class CategoryRepository : ICategoryRepository
{
    private readonly IDbConnectionFactory _db;
    public CategoryRepository(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<Category>> ListAsync()
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Category>("SELECT * FROM Categories WHERE Active=1 ORDER BY Name");
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Category>(
            "SELECT * FROM Categories WHERE Id=@Id AND Active=1", new { Id = id });
    }
}
