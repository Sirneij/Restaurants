using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Restaurants.Application.User;

namespace Restaurants.Application.Extensions;


public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly).AddFluentValidationAutoValidation();

        // Required for the UserContext to be injected into the controllers for the current user
        services.AddScoped<IUserContext, UserContext>();
        services.AddHttpContextAccessor();
    }
}