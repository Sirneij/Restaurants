using MediatR;
using Restaurants.Application.Restaurants.Dtos;

namespace Restaurants.Application.Restaurants.Queries.GetRestaurant;

public class GetRestaurantQuery(Guid Id) : IRequest<RestaurantDto?>
{
    public Guid Id { get; } = Id;
}
