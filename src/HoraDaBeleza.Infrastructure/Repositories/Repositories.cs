using Dapper;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Domain.Enums;
using HoraDaBeleza.Infrastructure.Data;

namespace HoraDaBeleza.Infrastructure.Repositories;

// ── User ───────────────────────────────────────────────────────────────────
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
            INSERT INTO Users (Name,Email,PasswordHash,Phone,PhotoUrl,Type,Active,CreatedAt)
            VALUES (@Name,@Email,@PasswordHash,@Phone,@PhotoUrl,@Type,@Active,@CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.QuerySingleAsync<int>(sql, user);
    }

    public async Task UpdateAsync(User user)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE Users SET Name=@Name,Phone=@Phone,PhotoUrl=@PhotoUrl,UpdatedAt=@UpdatedAt WHERE Id=@Id",
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

// ── Salon ──────────────────────────────────────────────────────────────────
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

// ── Professional ───────────────────────────────────────────────────────────
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

// ── Service ────────────────────────────────────────────────────────────────
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

// ── Appointment ────────────────────────────────────────────────────────────
public class AppointmentRepository : IAppointmentRepository
{
    private readonly IDbConnectionFactory _db;
    public AppointmentRepository(IDbConnectionFactory db) => _db = db;

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Appointment>(
            "SELECT * FROM Appointments WHERE Id=@Id", new { Id = id });
    }

    public async Task<IEnumerable<Appointment>> ListByClientAsync(int clientId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Appointment>(
            "SELECT * FROM Appointments WHERE ClientId=@ClientId ORDER BY ScheduledAt DESC",
            new { ClientId = clientId });
    }

    public async Task<IEnumerable<Appointment>> ListByProfessionalAsync(int professionalId, DateTime? date = null)
    {
        using var conn = _db.CreateConnection();
        var sql = "SELECT * FROM Appointments WHERE ProfessionalId=@ProfessionalId";
        if (date.HasValue) sql += " AND CAST(ScheduledAt AS DATE)=@Date";
        sql += " ORDER BY ScheduledAt";
        return await conn.QueryAsync<Appointment>(sql, new { ProfessionalId = professionalId, Date = date?.Date });
    }

    public async Task<IEnumerable<Appointment>> ListBySalonAsync(int salonId, DateTime? date = null)
    {
        using var conn = _db.CreateConnection();
        var sql = "SELECT * FROM Appointments WHERE SalonId=@SalonId AND Status NOT IN (3)";
        if (date.HasValue) sql += " AND CAST(ScheduledAt AS DATE)=@Date";
        sql += " ORDER BY ScheduledAt";
        return await conn.QueryAsync<Appointment>(sql, new { SalonId = salonId, Date = date?.Date });
    }

    public async Task<bool> HasConflictAsync(int professionalId, DateTime scheduledAt, int durationMinutes, int? ignoreId = null)
    {
        using var conn = _db.CreateConnection();
        var end  = scheduledAt.AddMinutes(durationMinutes);
        var sql  = @"SELECT COUNT(1) FROM Appointments
                    WHERE ProfessionalId=@ProfessionalId AND Status NOT IN (3,5)
                    AND (
                        (@Start >= ScheduledAt AND @Start < DATEADD(MINUTE,DurationMinutes,ScheduledAt))
                        OR (@End > ScheduledAt AND @End <= DATEADD(MINUTE,DurationMinutes,ScheduledAt))
                        OR (ScheduledAt >= @Start AND ScheduledAt < @End)
                    )";
        if (ignoreId.HasValue) sql += " AND Id<>@IgnoreId";
        return await conn.QuerySingleAsync<int>(sql,
            new { ProfessionalId = professionalId, Start = scheduledAt, End = end, IgnoreId = ignoreId }) > 0;
    }

    public async Task<int> CreateAsync(Appointment appointment)
    {
        using var conn = _db.CreateConnection();
        const string sql = @"
            INSERT INTO Appointments (ClientId,ProfessionalId,ServiceId,SalonId,ScheduledAt,
                DurationMinutes,TotalPrice,Status,Notes,CreatedAt)
            VALUES (@ClientId,@ProfessionalId,@ServiceId,@SalonId,@ScheduledAt,
                @DurationMinutes,@TotalPrice,@Status,@Notes,@CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.QuerySingleAsync<int>(sql, appointment);
    }

    public async Task UpdateStatusAsync(int id, AppointmentStatus status)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE Appointments SET Status=@Status,UpdatedAt=@UpdatedAt WHERE Id=@Id",
            new { Id = id, Status = (int)status, UpdatedAt = DateTime.UtcNow });
    }
}

// ── Review ─────────────────────────────────────────────────────────────────
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

// ── Plan ───────────────────────────────────────────────────────────────────
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

// ── Subscription ───────────────────────────────────────────────────────────
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

// ── Notification ───────────────────────────────────────────────────────────
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

// ── Category ───────────────────────────────────────────────────────────────
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
