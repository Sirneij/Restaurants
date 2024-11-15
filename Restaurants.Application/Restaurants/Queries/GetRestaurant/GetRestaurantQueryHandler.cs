using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Queries.GetRestaurant;
public class GetRestaurantQueryHandler(
    IRestaurantRepository restaurantRepository,
    ILogger<GetRestaurantQueryHandler> logger
) : IRequestHandler<GetRestaurantQuery, RestaurantDto?>
{

    public async Task<RestaurantDto?> Handle(GetRestaurantQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"GetRestaurantQueryHandler: Handling {request.Id}");
        var restaurant = await restaurantRepository.GetByIdAsync(request.Id);
        return restaurant is not null ? RestaurantDto.FromRestaurant(restaurant) : null;
    }
}