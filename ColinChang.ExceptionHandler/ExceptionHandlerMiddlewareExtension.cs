using ColinChang.ExceptionHandler.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ColinChang.ExceptionHandler
{
    public static class ExceptionHandlerMiddlewareExtension
    {
        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder app) =>
            app.UseErrorHandler(new ExceptionHandlerOptions());

        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder app,
            ExceptionHandlerOptions options)
        {
            app.UseMiddleware<ExceptionHandlerMiddleware>(options);
            return app;
        }

        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder app,
            RequestDelegate exceptionHandler)
        {
            app.UseMiddleware<ExceptionHandlerMiddleware>(exceptionHandler);
            return app;
        }
    }

    public class ExceptionHandlerOptions
    {
        /// <summary>
        /// 日志记录允许的Request.Body最大长度，超过后日志将记录OverSizeBodyLengthMessage内容
        /// </summary>
        public long LogMaxBodyLength { get; set; } = 4 * 1024;

        /// <summary>
        /// Request.Body长度超过LogMaxBodyLength后记录的错误消息
        /// </summary>
        public string OverSizeBodyLengthMessage { get; set; } = "the request body is too large to record";

        /// <summary>
        /// 发生异常后返回给客户端的响应对象
        /// </summary>
        public IOperationResult OperationResult { get; set; } =
            new OperationResult<object>(null, OperationException.DefaultMessage);
    }
}