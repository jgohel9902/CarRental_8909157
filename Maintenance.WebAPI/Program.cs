using Maintenance.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepairHistoryService, FakeRepairHistoryService>();

var usageCounts = new Dictionary<string, int>();
builder.Services.AddSingleton(usageCounts);

var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            error = "ServerError",
            message = "An unexpected error occurred."
        });
    }
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Maintenance.WebAPI v1");
    c.RoutePrefix = string.Empty;
});
app.MapGet("/swagger", () => Results.Redirect("/"));

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower() ?? "";

    bool isSwagger =
        path == "/" ||
        path.StartsWith("/swagger") ||
        path.StartsWith("/index.html") ||
        path.StartsWith("/favicon");

    if (isSwagger)
    {
        await next();
        return;
    }

    var expectedKey = app.Configuration["MaintenanceApi:ApiKey"] ?? "MY_SECRET_KEY_123";

    if (!context.Request.Headers.TryGetValue("X-Api-Key", out var key) || key != expectedKey)
    {
        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Unauthorized",
            message = "Missing or invalid API key."
        });
        return;
    }

    await next();
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();