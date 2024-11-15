using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Dishes.Commands.CreateDishes;

public class CreateDishCommandHandler(
    ILogger<CreateDishCommandHandler> logger,
    IDishRepository dishRepository
) : IRequestHandler<CreateDishCommand, Guid>
{
    public async Task<Guid> Handle(CreateDishCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating dish for restaurant with id {RestaurantId}", request.RestaurantId);
        Guid id = await dishRepository.CreateAsync(request);
        return id;
    }
}