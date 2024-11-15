using FluentValidation;

namespace Restaurants.Application.Restaurants.Commands.UpdateRestaurant;


public class UpdateRestaurantCommandValidator : AbstractValidator<UpdateRestaurantCommand>
{
    public UpdateRestaurantCommandValidator()
    {
        // Optional fields. If they are not provided, they will not be updated in the database
        RuleFor(r => r.Name)
            .MinimumLength(3)
            .MaximumLength(100)
            .WithMessage("The restaurant name must be between 3 and 100 characters long");

        RuleFor(r => r.Category)
            .MinimumLength(3)
            .MaximumLength(50)
            .WithMessage("The restaurant category must be between 3 and 50 characters long");

        RuleFor(r => r.ContactEmail)
            .EmailAddress()
            .WithMessage("The contact email is not valid");

        RuleFor(r => r.ContactPhone)
            .Matches(@"^\+?[0-9\s]{3,}$")
            .WithMessage("The contact phone is not valid");

    }
}
