using Restaurants.Domain.Entities;

namespace Restaurants.Domain.Repositories;

public interface IDishRepository
{
    Task<Guid> CreateAsync(Dish dish);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Dish>> GetAllAsync(Guid restaurantId);
    Task<Dish> GetByIdAsync(Guid id);
    Task UpdateAsync(Dish dish);
}