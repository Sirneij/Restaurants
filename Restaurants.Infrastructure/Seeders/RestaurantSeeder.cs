using Restaurants.Domain.Entities;
using Restaurants.Infrastructure.Persistence;

namespace Restaurants.Infrastructure.Seeders;

internal class RestaurantSeeder(RestaurantsDbContext dbContext) : IRestaurantSeeder
{
    public async Task Seed()
    {
        if (await dbContext.Database.CanConnectAsync())
        {
            if (!dbContext.Restaurants.Any())
            {
                var restaurants = GetRestaurants();
                await dbContext.Restaurants.AddRangeAsync(restaurants);
                await dbContext.SaveChangesAsync();
            };
        }
    }

    private IEnumerable<Restaurant> GetRestaurants()
    {
        // Generate restaurants called "Chipotle", "McDonalds", and "Burger King"
        // with random descriptions, categories, and addresses in the United States
        // and random dishes with random names and prices
        // Return the restaurants

        List<Restaurant> restaurants = [
            new(){
                Name = "Chipotle",
                Description = "Mexican Grill",
                Category = "Mexican",
                HasDelivery = true,
                ContactEmail = "",
                ContactPhone = "",
                Address = new(){
                    Street = "1234 Chipotle St",
                    City = "Chipotle City",
                    ZipCode = "12345",
                    Country = "United States"
                },
                Dishes = new(){
                    new(){
                        Name = "Burrito",
                        Description = "A burrito",
                        Price = 5.99m
                    },
                    new(){
                        Name = "Taco",
                        Description = "A taco",
                        Price = 2.99m
                    }
                }
            },
            new(){
                Name = "McDonalds",
                Description = "Fast Food",
                Category = "Fast Food",
                HasDelivery = true,
                ContactEmail = "",
                ContactPhone = "",
                Address = new(){
                    Street = "1234 McDonalds St",
                    City = "McDonalds City",
                    ZipCode = "12345",
                    Country = "United States"
                },
                Dishes = new(){
                    new(){
                        Name = "Big Mac",
                        Description = "A Big Mac",
                        Price = 3.99m
                    },
                    new(){
                        Name = "McNuggets",
                        Description = "McNuggets",
                        Price = 4.99m
                    }
                }
            },
            new(){
                Name = "Burger King",
                Description = "Fast Food",
                Category = "Fast Food",
                HasDelivery = true,
                ContactEmail = "",
                ContactPhone = "",
                Address = new(){
                    Street = "1234 Burger King St",
                    City = "Burger King City",
                    ZipCode = "12345",
                    Country = "United States"
                },
                Dishes = new(){
                    new(){
                        Name = "Whopper",
                        Description = "A Whopper",
                        Price = 4.99m
                    },
                    new(){
                        Name = "Chicken Fries",
                        Description = "Chicken Fries",
                        Price = 3.99m
                    }
                }
            }
        ];

        return restaurants;
    }
}