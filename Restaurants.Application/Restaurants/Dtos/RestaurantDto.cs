using Restaurants.Application.Dishes.Dtos;
using Restaurants.Domain.Entities;

namespace Restaurants.Application.Restaurants.Dtos;

public class RestaurantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Category { get; set; } = default!;
    public bool HasDelivery { get; set; }

    public Address? Address { get; set; }
    public List<DishDto> Dishes { get; set; } = [];

    public static RestaurantDto FromRestaurant(Restaurant restaurant)
    {
        return new RestaurantDto
        {
            Id = restaurant.Id,
            Name = restaurant.Name,
            Description = restaurant.Description,
            Category = restaurant.Category,
            HasDelivery = restaurant.HasDelivery,
            Address = restaurant.Address,
            Dishes = restaurant.Dishes.Select(DishDto.FromDish).ToList()
        };
    }
}