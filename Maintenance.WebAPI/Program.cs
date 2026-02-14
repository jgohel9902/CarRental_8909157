using Maintenance.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepairHistoryService, FakeRepairHistoryService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Maintenance.WebAPI v1");

    c.RoutePrefix = string.Empty;
});

app.MapGet("/swagger", () => Results.Redirect("/"));


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();