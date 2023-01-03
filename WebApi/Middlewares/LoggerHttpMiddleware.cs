using Microsoft.Extensions.Logging;

namespace WebApi.Middlewares
{
    public static class LoggerHttpMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggerHttp(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoggerHttpMiddleware>();
        }
    }

    public class LoggerHttpMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<LoggerHttpMiddleware> logger;

        public LoggerHttpMiddleware(
            RequestDelegate next, 
            ILogger<LoggerHttpMiddleware> logger
            )
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context) 
        {
            using (var ms = new MemoryStream())
            {
                var bodyOriginal = context.Response.Body;
                context.Response.Body = ms;

                await next( context  );
                ms.Seek(0, SeekOrigin.Begin);
                string response = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(bodyOriginal);
                context.Response.Body = bodyOriginal;

                logger.LogInformation(response);
            }
        }
    }
}
