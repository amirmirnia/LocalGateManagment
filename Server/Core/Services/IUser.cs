using ServicesGateManagment.Shared.Models.Common;
using ServicesGateManagment.Shared.Models.Users;
using ServicesGateManagment.Shared.Models.ViewModel.User;

namespace ServicesGateManagment.Server;

public interface IUser
{
    Task<bool> RegisterUser(User user);
    Task<ListUserDto> ListUser();
    Task<UserDto> GetUserById(int id);
}