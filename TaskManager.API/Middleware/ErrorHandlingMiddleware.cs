using System.Net;
using System.Text.Json;

namespace TaskManager.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); //   pipeline-а
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Възникна непредвидена грешка");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new
                {
                    error = "Възникна вътрешна грешка в сървъра.",
                    detail = ex.Message // За production можеш да го махнеш Подготовка за production: можеш лесно да скриеш detail в Release режим
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
