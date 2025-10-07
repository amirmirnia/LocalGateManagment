using AutoMapper;
using Azure;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServicesGateManagment.Server.Handlers;
using ServicesGateManagment.Shared;
using ServicesGateManagment.Shared.DBContext;
using ServicesGateManagment.Shared.Models.Common;
using ServicesGateManagment.Shared.Models.Users;
using ServicesGateManagment.Shared.Models.ViewModel.User;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace ServicesGateManagment.Server;

public class UserService : IUser
{
    private ApplicationDbContext _db;
    private readonly IMapper _mapper;


    public UserService(IConfiguration configuration, ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;

    }

    public async Task<bool> RegisterUser(User user)
    {
        try
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<ListUserDto> ListUser()
    {
        try
        {
            var listuser =  _db.Users.ToListAsync().Result;

            var finalmodel = _mapper.Map<List<UserDto>>(listuser);

            return new ListUserDto
            {
                ListUser = finalmodel
            };
        }
        catch (Exception ex)
        {
            // بهتره لاگ بگیری یا حداقل Exception رو نگه داری
            Console.WriteLine(ex.Message);
            return new ListUserDto();
        }
    }

    public async Task<UserDto> GetUserById(int id)
    {
        try
        {
            var user = _db.Users.Where(p => p.Id == id);

            if (user != null)
            {
                var finalmodel = _mapper.Map<UserDto>(user);

                return finalmodel;
            }
            return new UserDto();

        }
        catch (Exception ex)
        {
            // بهتره لاگ بگیری یا حداقل Exception رو نگه داری
            Console.WriteLine(ex.Message);
            return new UserDto();
        }
    }
}