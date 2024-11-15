using MediatR;
using Restaurants.Domain.Entities;

namespace Restaurants.Application.Restaurants.Commands.UpdateRestaurant;


public class UpdateRestaurantCommand : Restaurant, IRequest<bool> { }