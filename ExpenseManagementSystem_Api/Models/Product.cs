using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseManagementSystem_Api.Models
{
    public class Product
    {
        [Key]
        public int Product_Id { get; set; }

        public int Product_Quantity { get; set; }

        public int Product_Price { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Product_Date { get; set; }

        public int User_Id { get; set; }

    }
}
