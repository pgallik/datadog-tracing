namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.AspNetCore
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Tracing;

    public static class DataDogTracingApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDataDogTracing(
            this IApplicationBuilder app,
            TraceSource source,
            string serviceName = "web",
            Func<string, bool> shouldTracePathFunc = null)
        {
            if (shouldTracePathFunc == null)
                shouldTracePathFunc = x => true;

            app.Use(async (context, next) =>
            {
                var resource = context.Request.Host.Host;
                var path = context.Request.Path.HasValue ? context.Request.Path.Value : string.Empty;

                if (!shouldTracePathFunc(path))
                {
                    await next();
                    return;
                }

                using (var span = source.Begin("aspnet.request", serviceName, resource, "web"))
                using (new TraceContextScope(span))
                {
                    span.SetMeta("http.method", context.Request.Method);
                    span.SetMeta("http.path", path);
                    span.SetMeta("http.query", context.Request.QueryString.HasValue
                        ? context.Request.QueryString.Value
                        : string.Empty);

                    try
                    {
                        await next();
                    }
                    catch (Exception ex)
                    {
                        span.SetError(ex);
                        throw;
                    }

                    span.SetMeta("http.status_code", context.Response.StatusCode.ToString());
                }
            });

            return app;
        }
    }
}
