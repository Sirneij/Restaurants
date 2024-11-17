using MediatR;

namespace Restaurants.Application.Users.Commands.AssignRoles;
public class AssignUserRoleCommand : IRequest
{
    public string UserEmail { get; set; } = default!;
    public string RoleName { get; set; } = default!;
}
