using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Application.Users.Commands.AssignRoles;

public class AssignUserRoleCommandHandler(
    ILogger<AssignUserRoleCommandHandler> logger,
    UserManager<User> userManager
) : IRequestHandler<AssignUserRoleCommand>
{
    public async Task Handle(AssignUserRoleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Assigning role: {@Request}", request);

        var user = await userManager.FindByEmailAsync(request.UserEmail) ?? throw new NotFoundException(nameof(User), request.UserEmail);

        var result = await userManager.AddToRoleAsync(user, request.RoleName);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to assign role '{request.RoleName}' to user '{request.UserEmail}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}