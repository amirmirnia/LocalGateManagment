using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesGateManagment.Shared.Models.ViewModel.User
{
    public class ChangePasswordDto
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "پسورد جدید را وارد کنید")]
        [MinLength(6, ErrorMessage = "پسورد باید حداقل ۶ کاراکتر باشد")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "تکرار پسورد را وارد کنید")]
        public string ConfirmPassword { get; set; }
    }
}
