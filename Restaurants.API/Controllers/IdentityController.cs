using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Application.Users.Commands;

namespace Restaurants.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController(IMediator mediator) : ControllerBase
{
    [HttpPatch("users")]
    [Authorize]
    public async Task<IActionResult> UpdateDetails(UpdateUserDetailsCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }
}