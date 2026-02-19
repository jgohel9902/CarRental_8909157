var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Customer API HttpClient
builder.Services.AddHttpClient("CustomerApi", client =>
{
    var baseUrl = builder.Configuration["ApiSettings:CustomerApiBaseUrl"];
    client.BaseAddress = new Uri(baseUrl!);
});

// Maintenance API HttpClient
builder.Services.AddHttpClient("MaintenanceApi", (sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["MaintenanceApi:BaseUrl"]!);

    var apiKey = config["MaintenanceApi:ApiKey"] ?? "MY_SECRET_KEY_123";
    client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();