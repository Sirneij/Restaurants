using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants;

internal class RestaurantsService(IRestaurantRepository restaurantRepository, ILogger<RestaurantsService> logger) : IRestaurantsService
{
    public async Task<IEnumerable<RestaurantDto>> GetRestaurants()
    {
        logger.LogInformation("Getting all restaurants");
        var restaurants = await restaurantRepository.GetAllAsync();
        return restaurants.Select(RestaurantDto.FromRestaurant);
    }

    public async Task<RestaurantDto?> GetRestaurant(Guid id)
    {
        logger.LogInformation("Getting restaurant by ID: {Id}", id);
        var restaurant = await restaurantRepository.GetByIdAsync(id);
        return restaurant is not null ? RestaurantDto.FromRestaurant(restaurant) : null;
    }

}