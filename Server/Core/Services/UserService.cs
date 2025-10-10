using AutoMapper;
using Azure;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServicesGateManagment.Client.Pages;
using ServicesGateManagment.Server.Handlers;
using ServicesGateManagment.Shared;
using ServicesGateManagment.Shared.DBContext;
using ServicesGateManagment.Shared.Models.Common;
using ServicesGateManagment.Shared.Models.Enum;
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
            var user = _db.Users.FirstOrDefault(p => p.Id == id);

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

    public async Task<bool> DeleteUser(int id)
    {
        try
        {
            var user = _db.Users.FirstOrDefault(p=>p.Id==id);
            if (user!=null)
            {
                if (user.Role != UserRole.Admin)
                {
                    _db.Remove(user);
                    await _db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }
        catch (Exception)
        {

            return false;
        }
    }

    public async Task<bool> UpdateUser(User user)
    {
        try
        {
            var Model = _db.Users.FirstOrDefault(p => p.Id == user.Id);
            if (Model != null)
            {
                if (Model.Role != UserRole.Admin)
                {
                    Model.Role = user.Role;

                }
                Model.Email = user.Email;
                Model.FirstName = user.FirstName;
                Model.LastName = user.LastName;

                _db.Update(Model);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception)
        {

            return false;
        }
    }

    public async Task<User> GetUser(LoginDto user)
    {
        try
        {
            // جستجو بر اساس ایمیل
            var dbUser = await _db.Users.FirstOrDefaultAsync(p => p.Email == user.Username);

            // اگر کاربری پیدا شد، بررسی پسورد
            if (dbUser != null && PasswordHelper.VerifyPassword(user.Password, dbUser.Password))
            {
                return dbUser;
            }

            return null;
        }
        catch (Exception)
        {

            return null;
        }
    }

    public async Task<bool> ChangePassword(ChangePasswordDto PasswordModel)
    {
        try
        {
            var user = _db.Users.FirstOrDefault(p => p.Id == PasswordModel.UserId);
            if (user != null)
            {
                user.Password = PasswordHelper.HashPassword(PasswordModel.NewPassword);
                _db.Attach(user);
                _db.Entry(user).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception)
        {

            return false;
        }

    }
}