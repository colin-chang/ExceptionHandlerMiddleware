using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ColinChang.ExceptionHandler
{
    class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly Func<HttpContext, Exception, Task> _exceptionHandler;
        private readonly IOperationResult _operationResult;

        public ExceptionHandlerMiddleware(IOperationResult operationResult, RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _operationResult = operationResult;
        }

        public ExceptionHandlerMiddleware(Func<HttpContext, Exception, Task> exceptionHandler, RequestDelegate next)
        {
            _exceptionHandler = exceptionHandler;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                await (_exceptionHandler ?? HandleExceptionAsync).Invoke(context, e);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            string message;

            // expected exception
            if (exception is OperationException)
            {
                message = exception.Message;
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                _logger.LogWarning(exception, exception.Message);
            }
            // unexpected exception
            else
            {
                message = _operationResult.ErrorMessage;
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                _logger.LogError(exception, exception.Message);
            }

            _operationResult.ErrorMessage = message;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(_operationResult));
        }
    }
}