using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.Commands.DeleteRestaurant;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Commands.CreateRestaurant;

public class DeleteRestaurantCommandHandler(
    IRestaurantRepository restaurantRepository,
    ILogger<CreateRestaurantCommandHandler> logger
) : IRequestHandler<DeleteRestaurantCommand>
{

    public async Task Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting restaurant with id: {Id}", request.Id);
        await restaurantRepository.DeleteAsync(request.Id);
    }
}
