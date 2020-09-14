using System.Net;
using System.Threading.Tasks;
using ColinChang.ExceptionHandler.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ColinChang.ExceptionHandler
{
    public class OperationExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;
            var exception = context.Exception;
            var logger = context.HttpContext.RequestServices.GetService<ILogger<OperationExceptionFilterAttribute>>();
            var operationResult = context.HttpContext.RequestServices.GetService<IOperationResult>();

            string message;
            HttpStatusCode code;

            // expected exception
            if (exception is OperationException)
            {
                message = exception.Message;
                code = HttpStatusCode.BadRequest;
                logger.LogWarning(exception, exception.Message);
            }
            // unexpected exception
            else
            {
                message = operationResult.ErrorMessage;
                code = HttpStatusCode.InternalServerError;
                logger.LogError(exception, exception.Message);
            }

            operationResult.ErrorMessage = message;
            context.Result = new ObjectResult(JsonConvert.SerializeObject(operationResult,new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()})) {StatusCode = (int) code};
        }

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            OnException(context);
            await Task.CompletedTask;
        }
    }
}