using ServicesGateManagment.Shared;

namespace ServicesGateManagment.Client.Services
{
    public interface IVehicleService
    {
        Task<VehicleInquireResultVm> CreateVehicleInquire(CreateVehicleInquireRequest model);
        Task<VehicleInquireResultVm> CreateVehicleInquireApi(string EndPoint, CreateVehicleInquireRequest model);
    }
}
