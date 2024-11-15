using MediatR;
using Restaurants.Domain.Entities;

namespace Restaurants.Application.Restaurants.Commands.CreateRestaurant;


public class CreateRestaurantCommand : Restaurant, IRequest<Guid> { }