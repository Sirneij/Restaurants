using MediatR;
using Restaurants.Domain.Entities;

namespace Restaurants.Application.Dishes.Commands.UpdateDishes;

public class UpdateDishCommand : Dish, IRequest { }