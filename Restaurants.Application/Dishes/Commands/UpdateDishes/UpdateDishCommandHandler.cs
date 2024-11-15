using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Repositories;


namespace Restaurants.Application.Dishes.Commands.UpdateDishes;


public class UpdateDishCommandHandler(
    ILogger<UpdateDishCommandHandler> logger,
    IDishRepository dishRepository
) : IRequestHandler<UpdateDishCommand>
{
    public async Task Handle(UpdateDishCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating dish: {@request}", request);
        await dishRepository.UpdateAsync(request);
    }
}
