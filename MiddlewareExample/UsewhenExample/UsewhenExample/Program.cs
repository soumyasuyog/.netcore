var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWhen(
    context => context.Request.Query.ContainsKey("username"),
    app =>
    {
        app.Use(async (context, next) =>
        {
            await context.Response.WriteAsync("this is from middleware branch\n");
            await next();
        });
    }
    );

app.Run(async context =>
{
    await context.Response.WriteAsync("This is from main chain");
});

app.Run();
