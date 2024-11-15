using FluentValidation;

namespace Restaurants.Application.Restaurants.Commands.CreateRestaurant;

public class CreateRestaurantCommandValidator : AbstractValidator<CreateRestaurantCommand>
{
    public CreateRestaurantCommandValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100)
            .WithMessage("The restaurant name must be between 3 and 100 characters long");

        RuleFor(r => r.ContactEmail)
            .EmailAddress()
            .WithMessage("The contact email is not valid");

        RuleFor(r => r.ContactPhone)
            .Matches(@"^\+?[0-9\s]{3,}$")
            .WithMessage("The contact phone is not valid");
    }
}