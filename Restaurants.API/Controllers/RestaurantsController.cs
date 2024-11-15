using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;
using Restaurants.Application.Restaurants.Commands.DeleteRestaurant;
using Restaurants.Application.Restaurants.Queries.GetRestaurant;
using Restaurants.Application.Restaurants.Queries.GetRestaurants;

namespace Restaurants.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllRestaurants()
    {
        var restaurants = await mediator.Send(new GetRestaurantsQuery());
        return Ok(restaurants);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRestaurant([FromRoute] Guid id)
    {
        var restaurant = await mediator.Send(new GetRestaurantQuery(id));
        if (restaurant is null)
            return NotFound();

        return Ok(restaurant);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRestaurant([FromRoute] Guid id)
    {
        var isDeleted = await mediator.Send(new DeleteRestaurantCommand(id));
        if (!isDeleted)
            return NotFound();

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantCommand command)
    {
        Guid id = await mediator.Send(command);

        return CreatedAtAction(nameof(GetRestaurant), new { id }, null);
    }
}