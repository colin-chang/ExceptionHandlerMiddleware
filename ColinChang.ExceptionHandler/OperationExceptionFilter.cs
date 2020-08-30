using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ColinChang.ExceptionHandler
{
    public class OperationExceptionFilter : IExceptionFilter, IAsyncExceptionFilter
    {
        private readonly ILogger _logger;
        private readonly IOperationResult _operationResult;

        public OperationExceptionFilter(IOperationResult operationResult, ILogger<OperationExceptionFilter> logger)
        {
            _operationResult = operationResult;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;
            var exception = context.Exception;

            string message;
            HttpStatusCode code;

            // expected exception
            if (exception is OperationException)
            {
                message = exception.Message;
                code = HttpStatusCode.BadRequest;
                _logger.LogWarning(exception, exception.Message);
            }
            // unexpected exception
            else
            {
                message = _operationResult.ErrorMessage;
                code = HttpStatusCode.InternalServerError;
                _logger.LogError(exception, exception.Message);
            }

            _operationResult.ErrorMessage = message;
            context.Result = new ObjectResult(JsonConvert.SerializeObject(_operationResult)) {StatusCode = (int) code};
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            OnException(context);
            await Task.CompletedTask;
        }
    }
}