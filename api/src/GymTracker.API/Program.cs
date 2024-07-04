using GymTracker.API.Exceptions;
using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Authentication;
using GymTracker.Infrastructure.Common.Mapping;
using GymTracker.Infrastructure.Common.Utility;
using GymTracker.Infrastructure.Common.Utility.Interfaces;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Repositories;
using GymTracker.Infrastructure.Repositories.Interfaces;
using GymTracker.Infrastructure.Services;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace GymTracker.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Kestrel to listen on port 5000
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(5000);
        });

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("/app/logs/myapp.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Add services to the DI container
        ConfigureServices(builder.Services, builder.Configuration);

        builder.Host.UseSerilog();

        var app = builder.Build();

        // Apply database migrations
        ApplyMigrations(app);

        // Seed the database
        await SeedDatabase(app);

        // Configure the HTTP request pipeline
        ConfigureMiddleware(app);

        await app.RunAsync();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
                sqlOptions.MigrationsAssembly("GymTracker.Infrastructure");
            });
        });

        var jwtSection = configuration.GetSection("Jwt");
        var jwtSecret = jwtSection["Secret"];
        if (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Length < 16)
        {
            throw new ArgumentOutOfRangeException(nameof(jwtSecret), "JWT_SECRET must be at least 16 characters long.");
        }

        var key = Encoding.ASCII.GetBytes(jwtSecret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidAudience = jwtSection["Audience"],
            };
        })
        .AddGoogle(options =>
        {
            options.ClientId = configuration["Authentication:Google:ClientId"];
            options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
            options.CallbackPath = new PathString("/signin-google");
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

        // Add controllers
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddMemoryCache();

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWorkoutRepository, WorkoutRepository>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<ISeriesRepository, SeriesRepository>();
        services.AddScoped<IWorkoutTemplateRepository, WorkoutTemplateRepository>();
        services.AddScoped<ITemplateWeekRepository, TemplateWeekRepository>();
        services.AddScoped<ITemplateWorkoutRepository, TemplateWorkoutRepository>();
        services.AddScoped<ITemplateExerciseRepository, TemplateExerciseRepository>();
        services.AddScoped<IUserWorkoutTemplateRepository, UserWorkoutTemplateRepository>();
        services.AddScoped<IUserExerciseProgressRepository, UserExerciseProgressRepository>();

        // Add AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // Register services
        services.AddScoped<IExerciseService, ExerciseService>();
        services.AddScoped<ISeriesService, SeriesService>();
        services.AddScoped<IWorkoutService, WorkoutService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWorkoutTemplateService, WorkoutTemplateService>();
        services.AddScoped<IUserWorkoutTemplateService, UserWorkoutTemplateService>();
        services.AddScoped<IUserExerciseProgressService, UserExerciseProgressService>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IGuidValidator, GuidValidator>();
        services.AddScoped<IEntityValidator, EntityValidator>();
    }

    private static void ApplyMigrations(IHost app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while applying migrations.");
        }
    }

    private static async Task SeedDatabase(IHost app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var logger = services.GetRequiredService<ILogger<Program>>();
            var workoutTemplateService = services.GetRequiredService<IWorkoutTemplateService>();
            var dbContextLogger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

            ApplicationDbContext.SeedRoles(context, dbContextLogger);

            // Verifică și creează șabloanele de antrenament
            await SeedWorkoutTemplates(workoutTemplateService, logger);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    private static async Task SeedWorkoutTemplates(IWorkoutTemplateService workoutTemplateService, ILogger<Program> logger)
    {
        var templateFiles = new[]
        {
            "/app/WorkoutTemplatesJSON/fullbody.json",
        };

        logger.LogInformation($"Attempting to read template file from: {Path.GetFullPath(templateFiles[0])}");

        foreach (var filePath in templateFiles)
        {
            if (!File.Exists(filePath))
            {
                logger.LogWarning("Template file not found: {FilePath}", filePath);
                continue;
            }

            var existingTemplates = await workoutTemplateService.GetAllTemplatesAsync();
            var templateName = Path.GetFileNameWithoutExtension(filePath);

            if (!existingTemplates.Any(t => t.Name.Equals(templateName, StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    await workoutTemplateService.CreateWorkoutTemplateFromJsonFileAsync(filePath);
                    logger.LogInformation("Created workout template from file: {FilePath}", filePath);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error creating workout template from file: {FilePath}", filePath);
                }
            }
            else
            {
                logger.LogInformation("Workout template already exists: {TemplateName}", templateName);
            }
        }
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<ErrorHandlingMiddleware>();

        app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        app.Use(async (context, next) =>
        {
            await next.Invoke();
        });
        app.MapControllers();
    }
}