using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants;

internal class RestaurantsService(IRestaurantRepository restaurantRepository, ILogger<RestaurantsService> logger) : IRestaurantsService
{
    public async Task<IEnumerable<Restaurant>> GetRestaurants()
    {
        logger.LogInformation("Getting all restaurants");
        return await restaurantRepository.GetAllAsync();
    }

    public async Task<Restaurant?> GetRestaurant(Guid id)
    {
        logger.LogInformation("Getting restaurant by ID: {Id}", id);
        return await restaurantRepository.GetByIdAsync(id);
    }

}