using Microsoft.AspNetCore.Identity;

namespace Restaurants.Domain.Entities;
public class User : IdentityUser
{
    public DateOnly? BirthDate { get; set; }
    public string? Nationality { get; set; }
}
