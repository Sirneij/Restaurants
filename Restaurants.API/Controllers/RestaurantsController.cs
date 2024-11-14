using Microsoft.AspNetCore.Mvc;
using Restaurants.Application.Restaurants;
using Restaurants.Domain.Entities;

namespace Restaurants.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantsController(IRestaurantsService restaurantsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllRestaurants()
    {
        var restaurants = await restaurantsService.GetRestaurants();
        return Ok(restaurants);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRestaurant([FromRoute] Guid id)
    {
        var restaurant = await restaurantsService.GetRestaurant(id);
        if (restaurant is null)
            return NotFound();

        return Ok(restaurant);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRestaurant([FromBody] Restaurant restaurant)
    {
        Guid id = await restaurantsService.CreateRestaurant(restaurant);

        return CreatedAtAction(nameof(GetRestaurant), new { id }, null);
    }
}