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

    public async Task<IEnumerable<Professional>> GetTopAsync()
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Professional>(
            "SELECT p.* FROM Professionals p INNER JOIN Users u ON u.Id=p.UserId WHERE p.Active=1 ORDER BY p.AverageRating DESC");
    }

    public async Task<Application.DTOs.AvailabilityDto> GetAvailabilityAsync(int professionalId, string date)
    {
        using var conn = _db.CreateConnection();
        var sql = @"
            SELECT p.Id AS ProfessionalId, p.UserId, p.SalonId, p.Specialty, p.Bio, p.AverageRating, p.TotalReviews, p.Active,
                   s.Id AS SalonId, s.Name AS SalonName, s.Address AS SalonAddress, s.City AS SalonCity, s.State AS SalonState,
                   u.Name AS UserName, u.Base64Image AS UserPhoto
            FROM Professionals p
            INNER JOIN Salons s ON s.Id = p.SalonId
            INNER JOIN Users u ON u.Id = p.UserId
            WHERE p.Id = @ProfessionalId AND p.Active = 1";
        
        var result = await conn.QueryAsync(sql, new { ProfessionalId = professionalId });
        // Implementation would need to be completed based on actual requirements
        return new Application.DTOs.AvailabilityDto(professionalId, date, []);
    }

    public async Task<IEnumerable<Salon>> GetSalonsByProfessionalAsync(int professionalId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Salon>(
            "SELECT s.* FROM Salons s INNER JOIN Professionals p ON p.SalonId = s.Id WHERE p.UserId = @ProfessionalId AND s.Active = 1",
            new { ProfessionalId = professionalId });
    }
}
