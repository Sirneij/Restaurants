using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Entities;

namespace Restaurants.Application.Restaurants
{
    public interface IRestaurantsService
    {
        Task<IEnumerable<RestaurantDto>> GetRestaurants();
        Task<RestaurantDto?> GetRestaurant(Guid id);
        Task<Guid> CreateRestaurant(Restaurant restaurant);
    }
}