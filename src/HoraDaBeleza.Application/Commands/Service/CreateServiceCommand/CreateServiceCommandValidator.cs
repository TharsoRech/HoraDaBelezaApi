using FluentValidation;

namespace HoraDaBeleza.Application.Commands.Service.CreateServiceCommand;

public class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.DurationMinutes).GreaterThan(0).LessThanOrEqualTo(480);
    }
}
