using GymTracker.Infrastructure.Authentication;
using GymTracker.Infrastructure.Common.Mapping;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Repositories;
using GymTracker.Infrastructure.Repositories.Interfaces;
using GymTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GymTracker.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("GymTracker.Infrastructure")));

        var jwtSection = builder.Configuration.GetSection("Jwt");
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
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
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();
        builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();
        builder.Services.AddScoped<ISeriesRepository, SeriesRepository>();
        builder.Services.AddAutoMapper(typeof(MappingProfile));

        builder.Services.AddScoped<WorkoutService>();
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<JwtTokenService>();
        builder.Services.AddScoped<PasswordHasher>();

        var app = builder.Build();

        // Seed Roles
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                ApplicationDbContext.SeedRoles(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
            }
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
