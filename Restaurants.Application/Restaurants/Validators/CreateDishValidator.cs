using FluentValidation;
using Restaurants.Domain.Entities;

namespace Restaurants.Application.Restaurants.Validators;
public class CreateDishValidator : AbstractValidator<Dish>
{
    public CreateDishValidator()
    {
        RuleFor(d => d.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100)
            .WithMessage("The dish name must be between 3 and 100 characters long");

        RuleFor(d => d.Price)
            .GreaterThan(0)
            .WithMessage("The price must be greater than 0");

        RuleFor(d => d.RestaurantId)
            .NotEmpty()
            .WithMessage("The restaurant ID is required");
    }
}
