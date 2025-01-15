using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Models;

namespace ProductsApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Item> items { get; set; }

        public DbSet<Order> Orders { get; set; }

    }
}
