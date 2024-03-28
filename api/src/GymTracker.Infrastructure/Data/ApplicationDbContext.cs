using GymTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Workout> Workouts { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<Series> Series { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public static void SeedRoles(ApplicationDbContext context)
    {
        var roles = new List<Role>
            {
                new Role { Id = Guid.NewGuid(), Name = "Admin" },
                new Role { Id = Guid.NewGuid(), Name = "User" },
                new Role { Id = Guid.NewGuid(), Name = "Moderator" }
            };

        foreach (var role in roles)
        {
            if (!context.Roles.Any(r => r.Name == role.Name))
            {
                context.Roles.Add(role);
            }
        }
        context.SaveChanges();
    }
}