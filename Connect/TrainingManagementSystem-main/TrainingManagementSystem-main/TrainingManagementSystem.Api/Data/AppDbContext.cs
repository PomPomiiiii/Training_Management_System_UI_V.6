using Microsoft.EntityFrameworkCore;
using TrainingManagementSystem.Api.Entities;

namespace TrainingManagementSystem.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Attendee> Attendees { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    RoleId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                    Name = "Admin",
                    Description = "Administrator"
                },
                new Role
                {
                    RoleId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
                    Name = "User",
                    Description = "Regular User"
                }
            );
            
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);

            //one training with many materials, one materials with one training
            modelBuilder.Entity<Training>()
                .HasMany(t => t.Materials)
                .WithOne(m => m.Training)
                .HasForeignKey(m => m.TrainingId);

            //one training with many attendees, one attendee with one training
            modelBuilder.Entity<Training>()
                .HasMany(t => t.Attendees)
                .WithOne(a => a.Training)
                .HasForeignKey(a => a.TrainingId);
        }
    }
}
