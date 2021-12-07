using Microsoft.Extensions.Primitives;
using System.Net;

namespace MVCLocalization.Web.Middlewares
{
    public class CorrelationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private const string CorrelationIdHeaderKey = "X-Correlation-ID";


        public CorrelationMiddleware(RequestDelegate next,
             ILoggerFactory loggerFactory
            )
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<CorrelationMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //var header = context.Request.Headers[CorrelationIdHeaderKey];
            string correlationId = null;

            if (context.Request.Headers.TryGetValue(CorrelationIdHeaderKey, out StringValues correlationIds))
            {
                correlationId = correlationIds.FirstOrDefault(k => k.Equals(CorrelationIdHeaderKey));
                _logger.LogInformation($"CorrelationId from Request Header:{ correlationId}");
            }
            else
            {
                correlationId = Guid.NewGuid().ToString();
                context.Request.Headers.Add(CorrelationIdHeaderKey, correlationId);
                _logger.LogInformation($"Generated CorrelationId:{ correlationId}");
            }
            
            context.Response.OnStarting
            (
                () =>
                {
                    
                        if (
                            context.Response.StatusCode == (int)HttpStatusCode.OK &&
                            !context.Response.Headers.TryGetValue(CorrelationIdHeaderKey, out correlationIds)
                        )
                        {
                            context.Response.Headers.Add(CorrelationIdHeaderKey, correlationId);
                            _logger.LogInformation($"CorrelationId from Response Header:{ correlationId}");
                    }
                    return Task.CompletedTask;

                }
            );
            await _next.Invoke(context);

            //string sessionId = header.Any() ? header[0] : Guid.NewGuid().ToString();
            //context.Items["CorrelationId"] = sessionId;
            //await _next(context);
        }
    }

    public static class CorrelationMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorrelationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorrelationMiddleware>();
        }
    }


}
