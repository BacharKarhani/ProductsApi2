using Serilog;

namespace ProductsApi.Middlewares
{
    public class ResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalResponseBody = context.Response.Body;
            using (var memoryStream = new MemoryStream())
            {
                context.Response.Body = memoryStream;

                await _next(context);

                // Log response details
                Log.ForContext("ResponseStatusCode", context.Response.StatusCode)
                    .Information("Response sent");

                await memoryStream.CopyToAsync(originalResponseBody);
            }
        }
    }
}
