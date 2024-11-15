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

    public async Task<Guid> CreateAsync(Restaurant restaurant)
    {
        await dbContext.Restaurants.AddAsync(restaurant);
        await dbContext.SaveChangesAsync();
        return restaurant.Id;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var restaurant = new Restaurant { Id = id };
        dbContext.Restaurants.Attach(restaurant);
        dbContext.Restaurants.Remove(restaurant);

        try
        {
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
    }

}