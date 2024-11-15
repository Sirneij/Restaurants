using MediatR;
using Restaurants.Application.Dishes.Dtos;

namespace Restaurants.Application.Dishes.Queries.GetDishes;
public class GetDishesQuery(Guid restaurantId) : IRequest<IEnumerable<DishDto>>
{
    public Guid RestaurantId { get; } = restaurantId;
}
