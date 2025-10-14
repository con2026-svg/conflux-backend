using ConFlux.Data;
using ConFlux.DTOs;
using ConFlux.Model;
using ConFlux.Model.YourApp.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ConFlux.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // DTO za login zahtev
        public class LoginRequest
        {
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email i lozinka su obavezni.");

            // 🔹 Tražimo korisnika po Email-u
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

            if (user == null)
                return Unauthorized("Neispravan email ili lozinka.");

            // 🔹 Provera hash lozinke
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

            if (!computedHash.SequenceEqual(user.PasswordHash))
                return Unauthorized("Neispravan email ili lozinka.");

            // 🔹 Uzimamo role
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            // 🔹 JWT token generisanje
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new
            {
                Token = jwt,
                Email = user.Email,
                Roles = roles,
                Message = "Uspešno ste prijavljeni."
            });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email i lozinka su obavezni.");

            // 🔹 Proveri da li korisnik već postoji
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return Conflict("Korisnik sa ovom email adresom već postoji.");

            // 🔹 Hash i salt lozinke
            using var hmac = new HMACSHA512();
            var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
            var passwordSalt = hmac.Key;

            // 🔹 Kreiraj novog korisnika
            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // 🔹 Dodeli rolu (ako ne postoji, napravi)
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == request.Role);
            if (role == null)
            {
                role = new Role { Id = Guid.NewGuid(), Name = request.Role };
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
            }

            var userRole = new UserRole
            {
                UserId = newUser.Id,
                RoleId = role.Id
            };
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Korisnik uspešno registrovan.",
                Email = newUser.Email,
                Role = request.Role
            });
        }

    }
}

