using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Services.BackGround;
using ServicesGateManagment.Server;
using ServicesGateManagment.Shared.DBContext;
using YourProject.Services;
using IApiDataService = ServicesGateManagment.Server.IApiDataService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add HttpClient service
builder.Services.AddHttpClient();

builder.Services.AddScoped<IVehicleInquireService, VehicleInquireService>();
builder.Services.AddScoped<IApiDataService, ApiDataService>();
builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();

// Register the background service
builder.Services.AddHostedService<DataFetcherBackgroundService>();
builder.Services.AddHostedService<NetworkCheckBackgroundService>();


var app = builder.Build();


// ایجاد و به‌روزرسانی دیتابیس
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // اعمال Migrationها و ایجاد دیتابیس در صورت عدم وجود
        dbContext.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        // لاگ کردن خطا در صورت بروز مشکل
        Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
    }
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowBlazorClient");

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
