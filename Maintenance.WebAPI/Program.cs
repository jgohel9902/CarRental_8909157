using Microsoft.OpenApi.Models;
using Maintenance.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ Swagger + API Key support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Maintenance.WebAPI",
        Version = "v1"
    });

    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Enter API Key (header name: X-Api-Key). Example: MY_SECRET_KEY_123",
        In = ParameterLocation.Header,
        Name = "X-Api-Key",
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<IRepairHistoryService, FakeRepairHistoryService>();

// ✅ Stateful behavior
var usageCounts = new Dictionary<string, int>();
builder.Services.AddSingleton(usageCounts);

var app = builder.Build();

// ✅ Global exception handling middleware (must be first)
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

// ✅ Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Maintenance.WebAPI v1");
    c.RoutePrefix = string.Empty; // Swagger at /
});

// ✅ API Key middleware (allow swagger/static, protect API endpoints)
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower() ?? "";

    bool isSwaggerOrStatic =
        path == "/" ||
        path.StartsWith("/swagger") ||
        path.StartsWith("/index.html") ||
        path.StartsWith("/favicon") ||
        path.StartsWith("/swagger/");

    if (isSwaggerOrStatic)
    {
        await next();
        return;
    }

    // Pull from config first, fallback to hardcoded key
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