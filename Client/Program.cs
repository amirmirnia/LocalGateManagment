using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ServicesGateManagment.Client;
using ServicesGateManagment.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("ServicesGateManagment.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddScoped<ConfigService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServicesGateManagment.ServerAPI"));

// Add a general HttpClient for external API calls
builder.Services.AddHttpClient();

await builder.Build().RunAsync();
