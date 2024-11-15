using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Commands.UpdateRestaurant;
public class UpdateRestaurantCommandHandler(
    IRestaurantRepository restaurantRepository,
    ILogger<UpdateRestaurantCommandHandler> logger
) : IRequestHandler<UpdateRestaurantCommand, bool>
{
    public async Task<bool> Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating restaurant: {Id}", request.Id);
        // Build the restaurant object

        bool isUpdated = await restaurantRepository.UpdateAsync(request);
        if (!isUpdated)
        {
            logger.LogWarning("Restaurant with ID: {Id} not found", request.Id);
        }
        else
        {
            logger.LogInformation("Restaurant with ID: {Id} updated", request.Id);
        }
        return isUpdated;
    }
}
