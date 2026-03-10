using Dapper;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Infrastructure.Data;

namespace HoraDaBeleza.Infrastructure.Repositories;

public class PlanRepository : IPlanRepository
{
    private readonly IDbConnectionFactory _db;
    public PlanRepository(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<Plan>> ListActiveAsync()
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Plan>("SELECT * FROM Plans WHERE Active=1 ORDER BY Price");
    }

    public async Task<Plan?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Plan>(
            "SELECT * FROM Plans WHERE Id=@Id AND Active=1", new { Id = id });
    }
}
