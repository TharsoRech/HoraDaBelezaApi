using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListPopularSalonsQuery;

public class ListPopularSalonsQueryHandler(ISalonRepository repo, IReviewRepository reviewRepo, IAppointmentRepository appointmentRepo) : IRequestHandler<ListPopularSalonsQuery, IEnumerable<SalonDto>>
{

    public async Task<IEnumerable<SalonDto>> Handle(ListPopularSalonsQuery req, CancellationToken ct)
    {
        var salons = await repo.GetPopularAsync();
        var userId = req.UserId;
        
        return await Task.WhenAll(salons.Select(async s => await CreateSalonDto(s, userId)));
    }
    
    private async Task<SalonDto> CreateSalonDto(Salon s, int? userId)
    {
        // Calculate average rating and review count
        var reviews = await reviewRepo.GetReviewsAsync(s.Id);
        IEnumerable<Review> enumerable = reviews as Review[] ?? reviews.ToArray();
        var totalReviews = enumerable.Count();
        var averageRating = totalReviews > 0 ? enumerable.Average(r => r.Rating) : 0;
        var rating = totalReviews > 0 ? averageRating.ToString("F1") : "0";
        
        // Check if user has visited this salon
        var userHasVisited = false;
        if (userId.HasValue)
        {
            var appointments = await appointmentRepo.GetByUserIdAsync(userId.Value);
            userHasVisited = appointments.Any(a => a.SalonId == s.Id);
        }
        
        // Check if user is admin (owner)
        var isAdmin = userId.HasValue && s.OwnerId == userId.Value;
        
        return new SalonDto(
            s.Id, s.OwnerId, s.Name, s.Description, s.LogoUrl,
            s.Address, s.City, s.State, s.Phone, s.Latitude, s.Longitude, 
            s.AverageRating, s.Active, rating, totalReviews, s.WhatsApp, 
            s.Gallery, userHasVisited, s.Published, isAdmin);
    }
}
