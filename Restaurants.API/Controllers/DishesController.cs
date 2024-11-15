using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Application.Dishes.Commands.CreateDishes;
using Restaurants.Application.Dishes.Commands.DeleteDish;
using Restaurants.Application.Dishes.Commands.UpdateDishes;
using Restaurants.Application.Dishes.Dtos;
using Restaurants.Application.Dishes.Queries.GetDish;
using Restaurants.Application.Dishes.Queries.GetDishes;

namespace Restaurants.API.Controllers;

[ApiController]
[Route("api/restaurants/{restaurantId}/[controller]")]
public class DishesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DishDto>>> GetAllDishes([FromRoute] Guid restaurantId)
    {
        var dishes = await mediator.Send(new GetDishesQuery(restaurantId));
        return Ok(dishes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DishDto>> GetDish([FromRoute] Guid id)
    {
        var dish = await mediator.Send(new GetDishQuery(id));
        return Ok(dish);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDish([FromRoute] Guid id)
    {
        await mediator.Send(new DeleteDishCommand(id));
        return NoContent();
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDish([FromRoute] Guid restaurantId, [FromRoute] Guid id, [FromBody] UpdateDishCommand command)
    {
        command.Id = id;
        command.RestaurantId = restaurantId;
        await mediator.Send(command);
        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateDish([FromRoute] Guid restaurantId, [FromBody] CreateDishCommand command)
    {
        command.RestaurantId = restaurantId;
        Guid id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetDish), new { restaurantId, id }, null);
    }
}