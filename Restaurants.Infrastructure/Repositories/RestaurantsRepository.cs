using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Persistence;

namespace Restaurants.Infrastructure.Repositories;

internal class RestaurantsRepository(RestaurantsDbContext dbContext) : IRestaurantRepository
{
    public async Task<IEnumerable<Restaurant>> GetAllAsync()
    {
        return await dbContext.Restaurants
            .Include(r => r.Dishes) // Load related dishes for each restaurant
            .ToListAsync();
    }

    public async Task<Restaurant?> GetByIdAsync(Guid id)
    {
        return await dbContext.Restaurants
            .Include(r => r.Dishes) // Load related dishes for the specified restaurant
            .AsSplitQuery() // Use split queries to load related entities
            .FirstOrDefaultAsync(r => r.Id == id); // Retrieve the restaurant by its ID
    }

}