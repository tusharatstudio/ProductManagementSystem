using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseManagementSystem_Api.Models
{
    public class EMSDbContext : DbContext
    {
        public EMSDbContext(DbContextOptions<EMSDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        //public DbSet<Profile> Profiles { get; set; }
    }
}
