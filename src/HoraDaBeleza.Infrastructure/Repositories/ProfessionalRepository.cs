using Dapper;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Infrastructure.Data;

namespace HoraDaBeleza.Infrastructure.Repositories;

public class ProfessionalRepository : IProfessionalRepository
{
    private readonly IDbConnectionFactory _db;
    public ProfessionalRepository(IDbConnectionFactory db) => _db = db;

    public async Task<Professional?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Professional>(
            "SELECT * FROM Professionals WHERE Id=@Id AND Active=1", new { Id = id });
    }

    public async Task<IEnumerable<Professional>> ListBySalonAsync(int salonId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Professional>(
            "SELECT p.* FROM Professionals p INNER JOIN Users u ON u.Id=p.UserId WHERE p.SalonId=@SalonId AND p.Active=1",
            new { SalonId = salonId });
    }

    public async Task<int> CreateAsync(Professional professional)
    {
        using var conn = _db.CreateConnection();
        const string sql = @"
            INSERT INTO Professionals (UserId,SalonId,Specialty,Bio,AverageRating,TotalReviews,Active,CreatedAt)
            VALUES (@UserId,@SalonId,@Specialty,@Bio,@AverageRating,@TotalReviews,@Active,@CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.QuerySingleAsync<int>(sql, professional);
    }

    public async Task UpdateAsync(Professional professional)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE Professionals SET Specialty=@Specialty,Bio=@Bio WHERE Id=@Id", professional);
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("UPDATE Professionals SET Active=0 WHERE Id=@Id", new { Id = id });
    }
}
