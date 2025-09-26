using ServicesGateManagment.Shared;
using ServicesGateManagment.Shared.Models.ViewModel.Vehicles;

namespace ServicesGateManagment.Client.Services
{
    public interface IVehicleService
    {
        Task<VehicleInquireResultVm> CreateVehicleInquire(CreateVehicleInquireRequest model);
        Task<VehicleInquireResultVm> CreateVehicleInquireApi(string EndPoint, CreateVehicleInquireRequest model);
        Task<List<VehicleInquireRequestJsonVM>> GetAllRequestVehicle();
        Task<int> CountVehicleInFileJson();

    }
}
