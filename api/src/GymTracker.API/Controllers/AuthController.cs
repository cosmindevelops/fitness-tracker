using GymTracker.API.Models;
using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Authentication;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtTokenService _jwtTokenService;
        private readonly PasswordHasher _passwordHasher;

        public AuthController(ApplicationDbContext context, JwtTokenService jwtTokenService, PasswordHasher passwordHasher)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Username == model.Username || u.Email == model.Email);
            if (userExists)
            {
                return BadRequest("User already exists.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = model.Username,
                Email = model.Email,
                PasswordHash = _passwordHasher.HashPassword(null, model.Password)
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (defaultRole != null)
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = defaultRole.Id
                };
                await _context.UserRoles.AddAsync(userRole);
                await _context.SaveChangesAsync();
            }

            // Generarea token-ului ar trebui să considere rolurile utilizatorului
            var roles = new List<string> { defaultRole?.Name };
            var token = _jwtTokenService.GenerateToken(user, roles);

            return Ok(new { UserId = user.Id, Token = token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == model.Username);

            if (user == null)
            {
                return BadRequest("Invalid username or password.");
            }

            var verificationResult = _passwordHasher.VerifyPassword(user, model.Password, user.PasswordHash);

            if (verificationResult != PasswordVerificationResult.Success)
            {
                return BadRequest("Invalid username or password.");
            }

            var userRoles = await _context.UserRoles.Where(ur => ur.UserId == user.Id)
                                                     .Include(ur => ur.Role)
                                                     .Select(ur => ur.Role.Name)
                                                     .ToListAsync();

            var token = _jwtTokenService.GenerateToken(user, userRoles);

            return Ok(new { UserId = user.Id, Token = token });
        }

    }
}
