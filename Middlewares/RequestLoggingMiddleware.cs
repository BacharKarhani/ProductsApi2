using Serilog;

namespace ProductsApi.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Log.ForContext("RequestMethod", context.Request.Method)
                .ForContext("RequestPath", context.Request.Path)
                .Information("Request received");

            await _next(context);
        }
    }
}
