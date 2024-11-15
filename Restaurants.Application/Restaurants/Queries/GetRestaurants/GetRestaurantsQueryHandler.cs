using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Queries.GetRestaurants;

public class GetRestaurantsQueryHandler(
    IRestaurantRepository restaurantRepository,
    ILogger<GetRestaurantsQueryHandler> logger
) : IRequestHandler<GetRestaurantsQuery, IEnumerable<RestaurantDto>>
{


    public async Task<IEnumerable<RestaurantDto>> Handle(GetRestaurantsQuery request, CancellationToken cancellationToken)
    {

        logger.LogInformation("Getting all restaurants");
        var restaurants = await restaurantRepository.GetAllAsync();
        return restaurants.Select(RestaurantDto.FromRestaurant);

    }
}
