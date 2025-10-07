using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesGateManagment.Shared.Models.ViewModel.User
{
    public class ListUserDto
    {
        public ICollection<UserDto> ListUser { get; set; }
    }
}
