using MediatR;
using Restaurants.Application.Restaurants.Dtos;

namespace Restaurants.Application.Restaurants.Queries.GetRestaurants;

public class GetRestaurantsQuery : IRequest<IEnumerable<RestaurantDto>> { }
