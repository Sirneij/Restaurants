using MediatR;

namespace Restaurants.Application.Users.Commands.UpdateUserDetails;
public class UpdateUserDetailsCommand : IRequest
{
    public DateOnly? BirthDate { get; set; }
    public string? Nationality { get; set; }
}