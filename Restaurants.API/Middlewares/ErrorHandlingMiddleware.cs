using System.Net;
using Restaurants.Domain.Exceptions;

namespace Restaurants.API.Middlewares;
public class ErrorHandlingMiddleware(
    ILogger<ErrorHandlingMiddleware> logger
) : IMiddleware
{

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(ex, ex.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsJsonAsync(new
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {

                Message = "Something went wrong. Please try again later."
            });
        }
    }


}
