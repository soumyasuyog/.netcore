using MiddlewareExample.CustomMiddleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<MyCustomMiddleware>();
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");


//app.Run(async (HttpContext context) =>
//{
//    await context.Response.WriteAsync("Hello soumya");
//});


//Asp.Net core application-pipeline [or] middleware chain
//middleware 1
app.Use(async (HttpContext context, RequestDelegate next) =>
{
    await context.Response.WriteAsync("Hello\n");
    await next(context);
});

//middleware 2
//app.Use(async (HttpContext context, RequestDelegate next) =>
//{
//    await context.Response.WriteAsync("Hello");
//    await next(context);
//});

//new method for add middleware
//app.UseMiddleware<MyCustomMiddleware>();

// another new method for add middleware
//app.UseMyCustomMiddleware();

// another new method for add middleware
app.UseHelloCustomMiddleware();

//middleware 3
app.Run(async (HttpContext context) =>
{
    await context.Response.WriteAsync("Hello soumya\n");
});

app.Run();


//what is middleware extention - the middleware extention method is used to invoke the middleware with a single method call.