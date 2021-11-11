using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using bekami.Models;

namespace bekami.Data
{
    public class bekamiContext : DbContext
    {
        public bekamiContext (DbContextOptions<bekamiContext> options)
            : base(options)
        {
        }
        public DbSet<bekami.Models.User> User { get; set; }
        public DbSet<bekami.Models.Branch> Branch { get; set; }

        public DbSet<bekami.Models.Product> Product { get; set; }

        public DbSet<bekami.Models.Category> Category { get; set; }
        public DbSet<bekami.Models.Tags> Tags { get; set; }
        public DbSet<bekami.Models.Color> Color { get; set; }
        public DbSet<bekami.Models.OrderProduct> OrderProduct { get; set; }
        public DbSet<bekami.Models.Order> Order { get; set; }
        
        public DbSet<bekami.Models.Cart> Cart { get; set; }
        
        public DbSet<bekami.Models.CartItem> CartItem { get; set; }



    }
}
