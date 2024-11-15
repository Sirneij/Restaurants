using Restaurants.Application.Dishes.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Dishes.Queries.GetDish;

public class GetDishesQueryHandler(
    ILogger<GetDishesQueryHandler> logger,
    IDishRepository dishRepository
) : IRequestHandler<GetDishQuery, DishDto>
{
    public async Task<DishDto> Handle(GetDishQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting dish with id {Id}", request.Id);
        var dish = await dishRepository.GetByIdAsync(request.Id);
        return DishDto.FromDish(dish);
    }
}
