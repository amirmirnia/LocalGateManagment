using ServicesGateManagment.Shared;

namespace ServicesGateManagment.Server;

public interface IVehicleInquireService
{
    Task<VehicleInquireResultVm> ProcessInquireAsync(CreateVehicleInquireRequest request, CancellationToken cancellationToken = default);
}