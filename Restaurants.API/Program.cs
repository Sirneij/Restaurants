using Restaurants.Infrastructure.Extensions;
using Restaurants.Infrastructure.Seeders;
using Restaurants.Application.Extensions;
using Serilog;
using Restaurants.API.Middlewares;
using Restaurants.Domain.Entities;
using Restaurants.API.Extensions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddPresentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<IRestaurantSeeder>();

await seeder.Seed();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimingMiddleware>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGroup("api/identity").WithTags("Identity").MapIdentityApi<User>();

app.UseAuthorization();

app.MapControllers();

app.Run();
