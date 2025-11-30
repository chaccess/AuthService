using Application.Exceptions;

namespace Api.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BadRequestException ex)
            {
                await HandleException(context, ex, StatusCodes.Status400BadRequest);
            }
            catch (NotFoundException ex)
            {
                await HandleException(context, ex, StatusCodes.Status404NotFound);
            }
            catch (AlreadyExistsException ex)
            {
                await HandleException(context, ex, StatusCodes.Status409Conflict);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex, StatusCodes.Status500InternalServerError);
            }
        }

        private static async Task HandleException(HttpContext context, Exception exception, int statudCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statudCode;

            var res = new
            {
                Message = statudCode == StatusCodes.Status500InternalServerError ? "Внутренняя ошибка сервиса" : exception.Message,
                TraceId = context.TraceIdentifier,
            };

            await context.Response.WriteAsJsonAsync(res);
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
