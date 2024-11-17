using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Application.Users.Commands.UnassignRoles;

public class UnassignUserRoleCommandHandler(
    ILogger<UnassignUserRoleCommandHandler> logger,
    UserManager<User> userManager
) : IRequestHandler<UnassignUserRoleCommand>
{
    public async Task Handle(UnassignUserRoleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Unassigning role: {@Request}", request);

        var user = await userManager.FindByEmailAsync(request.UserEmail) ?? throw new NotFoundException(nameof(User), request.UserEmail);

        var result = await userManager.RemoveFromRoleAsync(user, request.RoleName);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to unassign role '{request.RoleName}' from user '{request.UserEmail}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}