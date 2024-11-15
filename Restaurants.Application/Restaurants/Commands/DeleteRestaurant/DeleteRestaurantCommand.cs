using MediatR;


namespace Restaurants.Application.Restaurants.Commands.DeleteRestaurant;

public class DeleteRestaurantCommand(Guid Id) : IRequest
{
    public Guid Id { get; } = Id;
}