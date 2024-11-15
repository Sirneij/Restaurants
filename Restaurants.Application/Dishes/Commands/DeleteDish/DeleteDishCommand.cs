using MediatR;


namespace Restaurants.Application.Dishes.Commands.DeleteDish;

public class DeleteDishCommand(Guid id) : IRequest
{
    public Guid Id { get; } = id;
}
