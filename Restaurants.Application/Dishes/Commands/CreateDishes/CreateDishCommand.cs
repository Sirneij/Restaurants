using MediatR;
using Restaurants.Domain.Entities;

namespace Restaurants.Application.Dishes.Commands.CreateDishes;

public class CreateDishCommand : Dish, IRequest<Guid> { }
