namespace Restaurants.Application.Dishes.Queries.GetDish;

using global::Restaurants.Application.Dishes.Dtos;
using MediatR;


public class GetDishQuery(Guid id) : IRequest<DishDto>
{
    public Guid Id { get; } = id;
}
