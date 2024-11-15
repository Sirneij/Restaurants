using FluentValidation;


namespace Restaurants.Application.Dishes.Commands.UpdateDishes;
public class UpdateDishCommandValidator : AbstractValidator<UpdateDishCommand>
{
    public UpdateDishCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .MinimumLength(3)
            .WithMessage("Name must be between 3 and 100 characters");

        RuleFor(x => x.Price)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0.00");

        RuleFor(x => x.KiloCalories)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("KiloCalories must be greater than 0");
    }
}
