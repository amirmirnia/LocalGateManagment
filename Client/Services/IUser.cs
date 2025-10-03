using ServicesGateManagment.Shared;
using ServicesGateManagment.Shared.Models.ViewModel.User;
using ServicesGateManagment.Shared.Models.ViewModel.Vehicles;

namespace ServicesGateManagment.Client.Services
{
    public interface IUser
    {
        Task<string> Login(LoginDto model);


    }
}
