var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


//Enable routing
app.UseRouting();


//Creating end points
//app.UseEndpoints(endpoint =>
//{
//    //add your end points
//    endpoint.Map("map1" , async (context) =>
//    {
//        await context.Response.WriteAsync("This is from map1");
//    });

//    endpoint.MapPost("map2", async (context) =>
//    {
//        await context.Response.WriteAsync("This is from map2");
//    });

//});.
app.UseEndpoints(endpoints =>
{
    endpoints.Map("files/{filename}.{extension}", async context =>
    {
        string? filename = Convert.ToString(context.Request.RouteValues["filename"]);
        string? extention = Convert.ToString(context.Request.RouteValues["extension"]);

        await context.Response.WriteAsync($"In files - {filename} - {extention}");
    });

    endpoints.Map("employee/profile/{employeename}", async context =>
    {
        string? employee = Convert.ToString(context.Request.RouteValues["employeename"]);
        await context.Response.WriteAsync($"In files - {employee}");
    });
});

app.Run(async context =>
{
    await context.Response.WriteAsync($"request recived at {context.Request.Path} ");
});
app.Run();
