using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Dishes.Commands.DeleteDish;


public class DeleteDishCommandHandler(
    ILogger<DeleteDishCommandHandler> logger,
    IDishRepository dishRepository
) : IRequestHandler<DeleteDishCommand>
{
    public async Task Handle(DeleteDishCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting dish with id {Id}", request.Id);
        await dishRepository.DeleteAsync(request.Id);
    }
}