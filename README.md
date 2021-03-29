# ExceptionHandler
A custom exception handler with a specific data model for asp.net core, including middleware and MVC exception filter.

## What this is about? 
Exceptions are usually divided into expected exceptions(`OperationException`) and unexpected exceptions. Expected exceptions are usually errors thrown manually by developers, such as invalid data validation, etc. Such exception messages are relatively safe and friendly and can be displayed directly to client users; Unexpected exceptions are unexpected program errors, such as logic bugs, database errors, etc. Such exception information usually contains sensitive information that requires the developer to intercept and process it and return a more user-friendly error message to the client users.

We providers an exception middleware, an exception filter, and an exception filter attribute that can help to handle exceptions in asp.net core web applications.

## Abstractions
### OperationException
A client-friendly exception type that can be used to show expected and safe information of the exception occurred.

### IOperationResult&lt;T&gt;
The actual return type for web requests with exceptions. We suggested you use this as the return type of all success requests, so whatever exceptions occurred or not, client users can always get responses with the same data structure. 

## How to use it?
### UseExceptionHandler
Custom exception middleware could help to handle exceptions in the middleware pipeline globally.

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // use middleware
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
```

### OperationExceptionFilter
`OperationExceptionFilter` can be used for asp.net core MVC application to handle exceptions of the framework.
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // inject IOperationResult for unexpected exception
    services.AddTransient<IOperationResult>(provider =>
        new OperationResult<object>(null, -1, OperationException.DefaultMessage));
    // inject filter globally  
    services.AddControllers(options => options.Filters.Add<OperationExceptionFilter>());
}
```

### OperationExceptionFilterAttribute
we could use `OperationExceptionFilterAttribute` in the same way as `OperationExceptionFilter` to handle MVC framework exceptions. Plus, it can also be used on Controllers and Actions as an Attribute which allows for more detailed control of exceptions.
#### global
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // inject IOperationResult for unexpected exception
    services.AddTransient<IOperationResult>(provider =>
        new OperationResult<object>(null, -1, OperationException.DefaultMessage));
    // inject filter globally  
    services.AddControllers(options => options.Filters.Add<OperationExceptionFilterAttribute>());
}
```
#### Controller/Action
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // inject IOperationResult for unexpected exception
    services.AddTransient<IOperationResult>(provider =>
        new OperationResult<object>(null, -1, OperationException.DefaultMessage));
    services.AddControllers();
}

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("{id}")]
    [OperationExceptionFilter]
    public Task<IOperationResult> GetAsync(int id)
    {
        if (id < 0)
            throw new OperationException("custom exception");

        if (id > 0)
            return Task.FromResult<IOperationResult>(new OperationResult<string>("success"));

        throw new Exception("unexpected exception");
    }
}
``` 

## Sample
[Sample](https://github.com/colin-chang/ExceptionHandler/tree/master/ColinChang.ExceptionHandler.Sameple) project shows how to use this middleware.

## Nuget
https://www.nuget.org/packages/ColinChang.ExceptionHandler/

## Docs
https://ccstudio.com.cn/dotnet/exception/introduction.html
