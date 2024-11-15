using MediatR;


namespace Restaurants.Application.Restaurants.Commands.DeleteRestaurant;

public class DeleteRestaurantCommand(Guid Id) : IRequest<bool>
{
    public Guid Id { get; } = Id;
}