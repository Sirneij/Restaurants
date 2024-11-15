using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Commands.UpdateRestaurant;
public class UpdateRestaurantCommandHandler(
    IRestaurantRepository restaurantRepository,
    ILogger<UpdateRestaurantCommandHandler> logger
) : IRequestHandler<UpdateRestaurantCommand>
{
    public async Task Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating restaurant: {@request}", request);

        await restaurantRepository.UpdateAsync(request);

    }
}
