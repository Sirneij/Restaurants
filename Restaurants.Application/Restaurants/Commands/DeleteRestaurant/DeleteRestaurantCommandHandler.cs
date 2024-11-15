using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.Commands.DeleteRestaurant;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Commands.CreateRestaurant;

public class DeleteRestaurantCommandHandler(
    IRestaurantRepository restaurantRepository,
    ILogger<CreateRestaurantCommandHandler> logger
) : IRequestHandler<DeleteRestaurantCommand, bool>
{

    public async Task<bool> Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
    {
        // Only one request should be sent to the database for this operation
        var deleted = await restaurantRepository.DeleteAsync(request.Id);
        if (!deleted)
        {
            logger.LogWarning("Restaurant with id {Id} was not found", request.Id);
        }

        return deleted;
    }
}
