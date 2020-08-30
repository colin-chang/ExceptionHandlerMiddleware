using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ColinChang.ExceptionHandler
{
    public static class ExceptionHandlerMiddlewareExtension
    {
        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            return app;
        }

        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder app,
            Func<HttpContext, Exception, Task> exceptionHandler)
        {
            app.UseMiddleware<ExceptionHandlerMiddleware>(exceptionHandler);
            return app;
        }
    }
}