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
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Add services to the DI container
        ConfigureServices(builder.Services, builder.Configuration);

        builder.Host.UseSerilog();

        var app = builder.Build();

        // Seed the database
        SeedDatabase(app);

        // Configure the HTTP request pipeline
        ConfigureMiddleware(app);

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("GymTracker.Infrastructure")));

        // Configure JWT authentication
        var jwtSection = configuration.GetSection("Jwt");
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSection["Secret"])),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidAudience = jwtSection["Audience"]
            };
        })
        .AddGoogle(options =>
        {
            var googleAuthNSection = configuration.GetSection("Authentication:Google");
            options.ClientId = googleAuthNSection["ClientId"];
            options.ClientSecret = googleAuthNSection["ClientSecret"];
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

        // Add AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // Register services
        services.AddScoped<IExerciseService, ExerciseService>();
        services.AddScoped<ISeriesService, SeriesService>();
        services.AddScoped<IWorkoutService, WorkoutService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IGuidValidator, GuidValidator>();
        services.AddScoped<IEntityValidator, EntityValidator>();
    }

    private static void SeedDatabase(IHost app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
            ApplicationDbContext.SeedRoles(context, logger);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
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

        app.MapControllers();
    }
}