using Microsoft.AspNetCore.ResponseCompression;
using Services.BackGround;
using ServicesGateManagment.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

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


var app = builder.Build();


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
