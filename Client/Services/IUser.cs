using ServicesGateManagment.Shared;
using ServicesGateManagment.Shared.Models.ViewModel.User;
using ServicesGateManagment.Shared.Models.ViewModel.Vehicles;

namespace ServicesGateManagment.Client.Services
{
    public interface IUser
    {
        Task<string> Login(LoginDto model);
        Task<bool> RegisterUser(RegisteUserDto registeUserDto);
        Task<ListUserDto> ListUser();
        Task<UserDto> GetUserById(int id); 

    }
}
