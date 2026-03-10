// ============================================================
// Service Commands
// ============================================================
using FluentValidation;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Services
{
    public record CreateServiceCommand(int SalonId, int OwnerId, int CategoryId, string Name,
        string? Description, decimal Price, int DurationMinutes) : IRequest<ServiceDto>;

    public class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
    {
        public CreateServiceCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.DurationMinutes).GreaterThan(0).LessThanOrEqualTo(480);
        }
    }

    public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, ServiceDto>
    {
        private readonly IServiceRepository  _repo;
        private readonly ISalonRepository    _salonRepo;
        private readonly ICategoryRepository _categoryRepo;

        public CreateServiceCommandHandler(IServiceRepository repo, ISalonRepository salonRepo, ICategoryRepository categoryRepo)
        { _repo = repo; _salonRepo = salonRepo; _categoryRepo = categoryRepo; }

        public async Task<ServiceDto> Handle(CreateServiceCommand req, CancellationToken ct)
        {
            var salon = await _salonRepo.GetByIdAsync(req.SalonId) ?? throw new NotFoundException("Salon", req.SalonId);
            if (salon.OwnerId != req.OwnerId) throw new UnauthorizedException();

            var category = await _categoryRepo.GetByIdAsync(req.CategoryId) ?? throw new NotFoundException("Category", req.CategoryId);

            var service = new Service
            {
                SalonId         = req.SalonId,
                CategoryId      = req.CategoryId,
                Name            = req.Name,
                Description     = req.Description,
                Price           = req.Price,
                DurationMinutes = req.DurationMinutes
            };

            var id = await _repo.CreateAsync(service);
            return new ServiceDto(id, req.SalonId, req.CategoryId, category.Name,
                req.Name, req.Description, req.Price, req.DurationMinutes, true);
        }
    }

    public record UpdateServiceCommand(int Id, int SalonId, int OwnerId, string Name,
        string? Description, decimal Price, int DurationMinutes, bool Active) : IRequest<Unit>;

    public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, Unit>
    {
        private readonly IServiceRepository _repo;
        private readonly ISalonRepository   _salonRepo;

        public UpdateServiceCommandHandler(IServiceRepository repo, ISalonRepository salonRepo)
        { _repo = repo; _salonRepo = salonRepo; }

        public async Task<Unit> Handle(UpdateServiceCommand req, CancellationToken ct)
        {
            var salon = await _salonRepo.GetByIdAsync(req.SalonId) ?? throw new NotFoundException("Salon", req.SalonId);
            if (salon.OwnerId != req.OwnerId) throw new UnauthorizedException();

            var service = await _repo.GetByIdAsync(req.Id) ?? throw new NotFoundException("Service", req.Id);
            service.Name            = req.Name;
            service.Description     = req.Description;
            service.Price           = req.Price;
            service.DurationMinutes = req.DurationMinutes;
            service.Active          = req.Active;

            await _repo.UpdateAsync(service);
            return Unit.Value;
        }
    }

    public record DeleteServiceCommand(int Id, int SalonId, int OwnerId) : IRequest<Unit>;

    public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, Unit>
    {
        private readonly IServiceRepository _repo;
        private readonly ISalonRepository   _salonRepo;

        public DeleteServiceCommandHandler(IServiceRepository repo, ISalonRepository salonRepo)
        { _repo = repo; _salonRepo = salonRepo; }

        public async Task<Unit> Handle(DeleteServiceCommand req, CancellationToken ct)
        {
            var salon = await _salonRepo.GetByIdAsync(req.SalonId) ?? throw new NotFoundException("Salon", req.SalonId);
            if (salon.OwnerId != req.OwnerId) throw new UnauthorizedException();
            await _repo.DeleteAsync(req.Id);
            return Unit.Value;
        }
    }

// ============================================================
// Professional Commands
// ============================================================
}

namespace HoraDaBeleza.Application.Commands.Professionals
{
    public record CreateProfessionalCommand(int UserId, int SalonId, int OwnerId,
        string? Specialty, string? Bio) : IRequest<ProfessionalDto>;

    public class CreateProfessionalCommandHandler : IRequestHandler<CreateProfessionalCommand, ProfessionalDto>
    {
        private readonly IProfessionalRepository _repo;
        private readonly ISalonRepository        _salonRepo;
        private readonly IUserRepository         _userRepo;

        public CreateProfessionalCommandHandler(IProfessionalRepository repo, ISalonRepository salonRepo, IUserRepository userRepo)
        { _repo = repo; _salonRepo = salonRepo; _userRepo = userRepo; }

        public async Task<ProfessionalDto> Handle(CreateProfessionalCommand req, CancellationToken ct)
        {
            var salon = await _salonRepo.GetByIdAsync(req.SalonId) ?? throw new NotFoundException("Salon", req.SalonId);
            if (salon.OwnerId != req.OwnerId) throw new UnauthorizedException();

            var user = await _userRepo.GetByIdAsync(req.UserId) ?? throw new NotFoundException("User", req.UserId);

            var professional = new Professional
            {
                UserId    = req.UserId,
                SalonId   = req.SalonId,
                Specialty = req.Specialty,
                Bio       = req.Bio
            };

            var id = await _repo.CreateAsync(professional);
            return new ProfessionalDto(id, req.UserId, req.SalonId, user.Name,
                user.PhotoUrl, req.Specialty, req.Bio, null, 0, true);
        }
    }

// ============================================================
// Review Commands
// ============================================================
}

namespace HoraDaBeleza.Application.Commands.Reviews
{
    public record CreateReviewCommand(int AppointmentId, int ClientId, int Rating, string? Comment) : IRequest<ReviewDto>;

    public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewCommandValidator()
        {
            RuleFor(x => x.Rating).InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
        }
    }

    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
    {
        private readonly IReviewRepository      _reviewRepo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IUserRepository        _userRepo;

        public CreateReviewCommandHandler(IReviewRepository reviewRepo,
            IAppointmentRepository appointmentRepo, IUserRepository userRepo)
        { _reviewRepo = reviewRepo; _appointmentRepo = appointmentRepo; _userRepo = userRepo; }

        public async Task<ReviewDto> Handle(CreateReviewCommand req, CancellationToken ct)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(req.AppointmentId)
                              ?? throw new NotFoundException("Appointment", req.AppointmentId);

            if (appointment.ClientId != req.ClientId)
                throw new UnauthorizedException("You cannot review this appointment.");

            if (appointment.Status != Domain.Enums.AppointmentStatus.Completed)
                throw new BusinessException("You can only review completed appointments.");

            if (await _reviewRepo.ReviewExistsForAppointmentAsync(req.AppointmentId))
                throw new BusinessException("This appointment has already been reviewed.");

            var client = await _userRepo.GetByIdAsync(req.ClientId);

            var review = new Review
            {
                AppointmentId  = req.AppointmentId,
                ClientId       = req.ClientId,
                ProfessionalId = appointment.ProfessionalId,
                SalonId        = appointment.SalonId,
                Rating         = req.Rating,
                Comment        = req.Comment
            };

            var id = await _reviewRepo.CreateAsync(review);
            return new ReviewDto(id, req.AppointmentId, client?.Name ?? "", req.Rating, req.Comment, DateTime.UtcNow);
        }
    }

// ============================================================
// Subscription Commands
// ============================================================
}

namespace HoraDaBeleza.Application.Commands.Subscriptions
{
    public record CreateSubscriptionCommand(int SalonId, int OwnerId, int PlanId) : IRequest<SubscriptionDto>;

    public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, SubscriptionDto>
    {
        private readonly ISubscriptionRepository _repo;
        private readonly ISalonRepository        _salonRepo;
        private readonly IPlanRepository         _planRepo;

        public CreateSubscriptionCommandHandler(ISubscriptionRepository repo,
            ISalonRepository salonRepo, IPlanRepository planRepo)
        { _repo = repo; _salonRepo = salonRepo; _planRepo = planRepo; }

        public async Task<SubscriptionDto> Handle(CreateSubscriptionCommand req, CancellationToken ct)
        {
            var salon = await _salonRepo.GetByIdAsync(req.SalonId) ?? throw new NotFoundException("Salon", req.SalonId);
            if (salon.OwnerId != req.OwnerId) throw new UnauthorizedException();

            var plan = await _planRepo.GetByIdAsync(req.PlanId) ?? throw new NotFoundException("Plan", req.PlanId);

            var active = await _repo.GetActiveBySalonAsync(req.SalonId);
            if (active != null)
            {
                active.Status = Domain.Enums.SubscriptionStatus.Cancelled;
                await _repo.UpdateAsync(active);
            }

            var now = DateTime.UtcNow;
            var subscription = new Subscription
            {
                SalonId   = req.SalonId,
                PlanId    = req.PlanId,
                Status    = Domain.Enums.SubscriptionStatus.Active,
                StartDate = now,
                EndDate   = now.AddDays(plan.PeriodDays)
            };

            var id = await _repo.CreateAsync(subscription);
            return new SubscriptionDto(id, req.SalonId, req.PlanId, plan.Name,
                Domain.Enums.SubscriptionStatus.Active, subscription.StartDate, subscription.EndDate);
        }
    }
}
