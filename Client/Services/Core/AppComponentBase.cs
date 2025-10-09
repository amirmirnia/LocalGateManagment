using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Extensions.Logging;
using Blazored.LocalStorage;

namespace ServicesGateManagment.Client.Services.Core
{
    public class AppComponentBase : ComponentBase
    {
        [Inject] protected IJSRuntime JSRuntime { get; set; }
        [Inject] protected IUser User { get; set; }
        [Inject] protected NavigationManager NavManager { get; set; }
        [Inject] protected NavigationManager Navigation { get; set; }
         
        [Inject] protected HttpClient Http { get; set; }
        [Inject] protected AlertService AlertService { get; set; }
        [Inject] protected IConfigService ConfigService { get; set; }
        [Inject] protected IVehicleService VehicleService { get; set; }
        [Inject] protected IApiDataService ApiDataService { get; set; }
        [Inject] protected ILogger<Index> Logger { get; set; }
        [Inject] protected CustomAuthStateProvider AuthStateProvider { get; set; }
        [Inject] protected IUser _User { get; set; }
        [Inject] protected ILocalStorageService LocalStorage { get; set; }
        [Inject] protected IVehicleService _vehicle { get; set; }
        [Inject] protected ConfirmService ConfirmService { get; set; }


    }
}

