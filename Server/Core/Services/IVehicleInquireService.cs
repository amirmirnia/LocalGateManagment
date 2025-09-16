using ServicesGateManagment.Shared;
using ServicesGateManagment.Shared.Enum;
using ServicesGateManagment.Shared.Vehicles;

namespace ServicesGateManagment.Server;

public interface IVehicleInquireService
{
    Task<VehicleInquireResultVm> ProcessInquireAsync(CreateVehicleInquireRequest request, CancellationToken cancellationToken = default);

    Task<VehicleJson> GetVehicleFromPlaque(string plaque);
    Task<VehiclePlateLetter> GetPlateLetter(string plaqueLetter);
}