using FluentValidation;
using Restaurants.Domain.Entities;

namespace Restaurants.Application.Restaurants.Validators;

public class CreateAddressValidator : AbstractValidator<Address>
{
    public CreateAddressValidator()
    {
        RuleFor(a => a.Street)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100)
            .WithMessage("The street name must be between 3 and 100 characters long");

        RuleFor(a => a.City)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100)
            .WithMessage("The city name must be between 3 and 100 characters long");

        RuleFor(a => a.ZipCode)
            .Matches(@"^\d{5}$")
            .WithMessage("The ZIP code is not valid");
    }
}