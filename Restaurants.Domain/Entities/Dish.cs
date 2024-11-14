namespace Restaurants.Domain.Entities;

public class Dish
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public Guid RestaurantId { get; set; }

    public float? KiloCalories { get; set; }
}
