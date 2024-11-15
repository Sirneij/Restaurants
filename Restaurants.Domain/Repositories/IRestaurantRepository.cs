using Restaurants.Domain.Entities;

namespace Restaurants.Domain.Repositories;

public interface IRestaurantRepository
{
    Task<Guid> CreateAsync(Restaurant restaurant);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Restaurant>> GetAllAsync();
    Task<Restaurant?> GetByIdAsync(Guid id);

}
