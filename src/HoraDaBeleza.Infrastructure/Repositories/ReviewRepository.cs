using Dapper;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Infrastructure.Data;

namespace HoraDaBeleza.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly IDbConnectionFactory _db;
    public ReviewRepository(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<Review>> ListBySalonAsync(int salonId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Review>(
            "SELECT * FROM Reviews WHERE SalonId=@SalonId ORDER BY CreatedAt DESC", new { SalonId = salonId });
    }

    public async Task<IEnumerable<Review>> ListByProfessionalAsync(int professionalId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Review>(
            "SELECT * FROM Reviews WHERE ProfessionalId=@ProfessionalId ORDER BY CreatedAt DESC",
            new { ProfessionalId = professionalId });
    }

    public async Task<bool> ReviewExistsForAppointmentAsync(int appointmentId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QuerySingleAsync<int>(
            "SELECT COUNT(1) FROM Reviews WHERE AppointmentId=@AppointmentId",
            new { AppointmentId = appointmentId }) > 0;
    }

    public async Task<int> CreateAsync(Review review)
    {
        using var conn = _db.CreateConnection();
        const string sql = @"
            INSERT INTO Reviews (AppointmentId,ClientId,ProfessionalId,SalonId,Rating,Comment,CreatedAt)
            VALUES (@AppointmentId,@ClientId,@ProfessionalId,@SalonId,@Rating,@Comment,@CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            UPDATE Professionals SET
                AverageRating=(SELECT AVG(CAST(Rating AS DECIMAL(3,2))) FROM Reviews WHERE ProfessionalId=@ProfessionalId),
                TotalReviews=(SELECT COUNT(1) FROM Reviews WHERE ProfessionalId=@ProfessionalId)
            WHERE Id=@ProfessionalId;";
        return await conn.QuerySingleAsync<int>(sql, review);
    }
}
