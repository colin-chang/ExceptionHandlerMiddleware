using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ColinChang.ExceptionHandler.Abstractions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ColinChang.ExceptionHandler
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly RequestDelegate _exceptionHandler;
        private readonly IOperationResult _operationResult;
        private readonly long _logMaxBodyLength;
        private readonly string _overSizeBodyLengthMessage;

        public ExceptionHandlerMiddleware(ExceptionHandlerOptions options, RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger)
        {
            _logMaxBodyLength = options.LogMaxBodyLength;
            _overSizeBodyLengthMessage = options.OverSizeBodyLengthMessage;
            _operationResult = options.OperationResult;

            _next = next;
            _logger = logger;
        }

        public ExceptionHandlerMiddleware(RequestDelegate exceptionHandler, RequestDelegate next)
        {
            _exceptionHandler =
                exceptionHandler ?? throw new ArgumentNullException($"{nameof(exceptionHandler)} cannot be null");

            _next = next;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                //允许Request.Body多次读取
                context.Request.EnableBuffering();
                await _next(context);
            }
            catch (Exception e)
            {
                context.Features.Set<IExceptionHandlerPathFeature>(new ExceptionHandlerFeature {Error = e});
                await (_exceptionHandler ?? HandleErrorAsync).Invoke(context);
            }
        }

        private async Task HandleErrorAsync(HttpContext context)
        {
            var error = context.Features.Get<IExceptionHandlerPathFeature>().Error;
            if (error == null)
                return;

            // 解析请求参数
            string body;
            if (context.Request.HasFormContentType)
            {
                var files = context.Request.Form.Files.Select(f =>
                    new KeyValuePair<string, string>($"{f.Name}(file)", f.FileName));
                var dict = new Dictionary<string, string>(files);
                foreach (var (k, v) in context.Request.Form)
                    dict[k] = v;
                body = JsonConvert.SerializeObject(dict);
            }
            else
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, context.Request.ContentType == null
                    ? Encoding.UTF8
                    : new MediaType(context.Request.ContentType).Encoding);
                var request = await reader.ReadToEndAsync();
                context.Request.Body.Seek(0, SeekOrigin.Begin);
                body = request.Length > _logMaxBodyLength ? _overSizeBodyLengthMessage : request;
            }

            var log = JsonConvert.SerializeObject(new
            {
                Url = context.Request.GetEncodedUrl(),
                context.Request.Method,
                context.Request.Headers,
                context.Request.Cookies,
                context.Request.Query,
                Body = body
            });

            string message;
            const string logTemplate = "error:{0}\r\nrequest{1}";
            // expected exception
            if (error is OperationException)
            {
                message = error.Message;
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                _logger.LogWarning(error, logTemplate, error.Message, log);
            }
            // unexpected exception
            else
            {
                message = _operationResult.ErrorMessage;
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                _logger.LogError(error, logTemplate, error.Message, log);
            }

            context.Response.ContentType = "application/json";
            _operationResult.ErrorMessage = message;
            _operationResult.Code = context.Response.StatusCode;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(_operationResult,
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()}));
        }
    }
}