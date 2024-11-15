using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Commands.CreateRestaurant;

public class CreateRestaurantCommandHandler(
    IRestaurantRepository restaurantRepository,
    ILogger<CreateRestaurantCommandHandler> logger
) : IRequestHandler<CreateRestaurantCommand, Guid>
{

    public async Task<Guid> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating restaurant: {@request}", request);
        Guid Id = await restaurantRepository.CreateAsync(request);
        return Id;
    }
}
