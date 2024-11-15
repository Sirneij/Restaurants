using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Persistence;

namespace Restaurants.Infrastructure.Repositories;
internal class DishesRepository(RestaurantsDbContext dbContext) : IDishRepository
{


    public async Task<Guid> CreateAsync(Dish dish)
    {
        await dbContext.Dishes.AddAsync(dish);
        await dbContext.SaveChangesAsync();

        return dish.Id;
    }

    public async Task DeleteAsync(Guid id)
    {
        var dish = new Dish { Id = id };
        dbContext.Dishes.Attach(dish);
        dbContext.Dishes.Remove(dish);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new NotFoundException(nameof(Dish), id.ToString());
        }
    }

    public async Task<IEnumerable<Dish>> GetAllAsync(Guid restaurantId)
    {
        return await dbContext.Dishes
            .Where(d => d.RestaurantId == restaurantId)
            .ToListAsync();
    }

    public async Task<Dish> GetByIdAsync(Guid id)
    {
        return await dbContext.Dishes.FirstOrDefaultAsync(d => d.Id == id) ?? throw new NotFoundException(nameof(Dish), id.ToString());
    }

    public async Task UpdateAsync(Dish dish)
    {
        dbContext.Dishes.Update(dish);
        await dbContext.SaveChangesAsync();
    }
}
