using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Services.BackGround;
using ServicesGateManagment.Server;
using ServicesGateManagment.Server.Core;
using ServicesGateManagment.Server.Handlers;
using ServicesGateManagment.Shared.DBContext;
using ServicesGateManagment.Shared.Models.Mapping;
using System.Text;
using YourProject.Services;
using IApiDataService = ServicesGateManagment.Server.IApiDataService;

var builder = WebApplication.CreateBuilder(args);

var apiKey = builder.Configuration.GetValue<string>("ApiSquretySettings:ApiKey");
var TargetUrl = builder.Configuration.GetValue<string>("ApiSettings:TargetUrl");

builder.Services.AddAutoMapper(typeof(Mapping).Assembly);
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
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri($"{TargetUrl}/"); // آدرس پایه API
    client.DefaultRequestHeaders.Add("X-API-Key", apiKey); // کلید API
});

builder.Services.AddSingleton<ApiService>();

builder.Services.AddScoped<IVehicleInquireService, VehicleInquireService>();
builder.Services.AddScoped<IApiDataService, ApiDataService>();
builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<JwtTokenService>();

// Register the background service
builder.Services.AddHostedService<DataFetcherBackgroundService>();
builder.Services.AddHostedService<NetworkCheckBackgroundService>();



// اضافه کردن Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

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
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowBlazorClient");

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
