using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{/*
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        // public DbSet<Role> Roles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            // modelBuilder.Entity<Role>().ToTable("roles");
        }
        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("users")
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasColumnName("id_user")
                .ValueGeneratedOnAdd(); // Указываем, что id_user - автоинкрементируемый

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasColumnName("id_role");

            base.OnModelCreating(modelBuilder);
        }
    }*/
}

