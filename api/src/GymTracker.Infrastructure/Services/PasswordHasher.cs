using GymTracker.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace GymTracker.Infrastructure.Services
{
    public class PasswordHasher
    {
        private readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public PasswordVerificationResult VerifyPassword(User user, string providedPassword, string storedHash)
        {
            // Corectăm ordinea parametrilor aici
            return _passwordHasher.VerifyHashedPassword(user, storedHash, providedPassword);
        }
    }
}
