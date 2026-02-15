using Microsoft.EntityFrameworkCore;
using BackEnd.Models;

namespace BackEnd.Context
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
    }
}
