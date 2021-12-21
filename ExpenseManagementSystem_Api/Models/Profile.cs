using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseManagementSystem_Api.Models
{
    public class Profile
    {
        [Display(Name = "Profile Picture")]
        public byte[] ProfilePicture { get; set; }

        [StringLength(40)]
        public string Password { get; set; }

        [StringLength(40)]
        public string Email_address { get; set; }
    }
}
