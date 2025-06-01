using LinkBioAPI.Data;
using LinkBioAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LinkBioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // In-memory user store for demo purposes
        private static ConcurrentDictionary<string, User> users = new();

        private readonly IConfiguration _config;
        private readonly AppDbContext _context;
        public AuthController(IConfiguration config, AppDbContext context)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            if (_context.Users.Any(u => u.Username == request.Username))
                return BadRequest("User already exists.");

            var salt = GenerateSalt();
            var hash = HashPassword(request.Password, salt);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = Convert.FromBase64String(hash),
                PasswordSalt = salt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }
        // Sign In Method - Retrieve User from SQL
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
                return Unauthorized("Invalid credentials.");
            Console.WriteLine($"User found: {user.Username}");
            var hash = HashPassword(request.Password, user.PasswordSalt);
            if (hash != Convert.ToBase64String(user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            var token = GenerateJwtToken(user.Username);
            return Ok(new { token });
        }
        private string GenerateJwtToken(string username)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static byte[] GenerateSalt()
        {
            var saltBytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);
            return saltBytes;
            // return Convert.ToBase64String(saltBytes);
        }

        private static string HashPassword(string password, byte[] salt)
        {
            using var sha256 = SHA256.Create();
            string saltString = Convert.ToBase64String(salt);
            var saltedPassword = Encoding.UTF8.GetBytes(password + saltString);
            var hash = sha256.ComputeHash(saltedPassword);
            return Convert.ToBase64String(hash);
        }


        // DTOs and User model

        // public class User
        // {
        //     public string Username { get; set; }
        //     public string PasswordHash { get; set; }
        //     public string Salt { get; set; }
        // }
    }
}