using ColinChang.ExceptionHandler.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ColinChang.ExceptionHandler.Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IOperationResult>(_ =>
                new OperationResult<object>(null, -1, OperationException.DefaultMessage));

            services.AddControllers();
            // services.AddControllers(options => options.Filters.Add<OperationExceptionFilter>());
            // services.AddControllers(options => options.Filters.Add<OperationExceptionFilterAttribute>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseErrorHandler();
            // app.UseErrorHandler(async (context, e) => await context.Response.WriteAsync("unexpected exception"));

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}