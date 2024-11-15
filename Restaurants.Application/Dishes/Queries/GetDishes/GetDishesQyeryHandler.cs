using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Dishes.Dtos;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Dishes.Queries.GetDishes;

public class GetDishesQueryHandler(
    ILogger<GetDishesQueryHandler> logger,
    IDishRepository dishRepository
) : IRequestHandler<GetDishesQuery, IEnumerable<DishDto>>
{

    public async Task<IEnumerable<DishDto>> Handle(GetDishesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Gettings dishes for restaurant with id {RestaurantId}", request.RestaurantId);
        var dishes = await dishRepository.GetAllAsync(request.RestaurantId);
        return dishes.Select(DishDto.FromDish);
    }
}
