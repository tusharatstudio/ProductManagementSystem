using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseManagementSystem_Api.Models
{
    public class User
    {
        [Key]
        public int User_Id { get; set; }

        [StringLength(50)]
        [Display(Name="FullName")]
        public string Full_Name { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage ="Please enter valid email")]
        public string Email_Id { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Please enter valid password")]
        public string Password { get; set; }
    }
}
