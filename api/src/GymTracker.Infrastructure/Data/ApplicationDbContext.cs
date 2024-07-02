using GymTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private readonly ILogger<ApplicationDbContext> _logger;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger<ApplicationDbContext> logger) : base(options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Workout> Workouts { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<Series> Series { get; set; }
    public DbSet<WorkoutTemplate> WorkoutTemplates { get; set; }
    public DbSet<TemplateExercise> TemplateExercises { get; set; }
    public DbSet<TemplateWeek> TemplateWeeks { get; set; }
    public DbSet<TemplateWorkout> TemplateWorkouts { get; set; }
    public DbSet<UserWorkoutTemplate> UserWorkoutTemplates { get; set; }
    public DbSet<UserExerciseProgress> UserExerciseProgresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public static void SeedRoles(ApplicationDbContext context, ILogger<ApplicationDbContext> logger)
    {
        try
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
                    logger.LogInformation("Seeding role: {RoleName}", role.Name);
                }
            }
            context.SaveChanges();
            logger.LogInformation("Roles seeded successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding roles.");
            throw;
        }
    }
}