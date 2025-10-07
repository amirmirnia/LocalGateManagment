using AutoMapper;
using ServicesGateManagment.Shared.Models.Users;
using ServicesGateManagment.Shared.Models.Vehicles;
using ServicesGateManagment.Shared.Models.ViewModel.User;
using ServicesGateManagment.Shared.Models.ViewModel.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesGateManagment.Shared.Models.Mapping
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<VehicleInquireRequestJson, VehicleInquireRequestJsonVM>();
            CreateMap<RegisteUserDto, User>();
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<UpdateUserDto, User>();
        }
    }
}
