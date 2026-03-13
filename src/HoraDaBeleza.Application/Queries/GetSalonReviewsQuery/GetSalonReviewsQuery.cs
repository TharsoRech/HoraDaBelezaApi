using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.GetSalonReviewsQuery;

public record GetSalonReviewsQuery(int SalonId) : IRequest<IEnumerable<ReviewDto>>;