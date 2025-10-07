
using ServicesGateManagment.Shared.Common;
using ServicesGateManagment.Shared.Models.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace ServicesGateManagment.Shared.Models.Users
{
    public class User : BaseAuditableEntity
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        public UserRole Role { get; set; }
    }
}
