using AuthFunctions.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthFunctions.Data.Contexts
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToContainer("AuthFunctions-Users")
                .HasPartitionKey(u => u.Id);
            
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();
            
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .OwnsMany(u => u.Roles);

            modelBuilder.Entity<User>()
                .OwnsOne(u => u.Profile);
        }
    }
}
