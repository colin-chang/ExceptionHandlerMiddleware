using ColinChang.ExceptionHandler.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ColinChang.ExceptionHandler.Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // services.AddControllers(options => options.Filters.Add<OperationExceptionFilter>());
            // services.AddControllers(options => options.Filters.Add<OperationExceptionFilterAttribute>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseErrorHandler();
            // app.UseErrorHandler(new ErrorHandlerOptions
            // {
            //     LogMaxBodyLength = 1024,
            //     OverSizeBodyLengthMessage = "request body oversize",
            //     OperationResult = new OperationResult<int>(-1, "error occurs")
            // });
            // app.UseErrorHandler(async context =>
            // {
            //     var error = context.Features.Get<IExceptionHandlerPathFeature>().Error;
            //     await context.Response.WriteAsync($"unexpected exception:{error.Message}");
            // });

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}