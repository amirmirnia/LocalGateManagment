using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ServicesGateManagment.Client;
using ServicesGateManagment.Client.Services;
using ServicesGateManagment.Client.Services.Core;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("ServicesGateManagment.ServerAPI", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);

    // 🔽 این بخش را اضافه کن:
    client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
    {
        NoCache = true,
        NoStore = true,
        MustRevalidate = true
    };
});
builder.Services.AddSingleton<ConfirmService>();
builder.Services.AddSingleton<AlertService>();
builder.Services.AddScoped<IConfigService, ConfigService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IApiDataService, ApiDataService>();
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<CustomAuthStateProvider>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServicesGateManagment.ServerAPI"));

// Add a general HttpClient for external API calls
builder.Services.AddHttpClient();
await builder.Build().RunAsync();
