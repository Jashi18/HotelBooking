using HotelBooking.Application.Interfaces;
using HotelBooking.Application.Settings;
using HotelBooking.Domain.Data;
using HotelBooking.Domain.Entities;
using HotelBooking.Models.AuthModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelBooking.Application.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            ApplicationDbContext context,
            IPasswordService passwordService,
            IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _passwordService = passwordService;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.DeleteDate == null);

            if (user == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            if (!_passwordService.VerifyPasswordHash(request.Password, user.PasswordHash, user.Salt))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid password"
                };
            }

            // Generate token
            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Authentication successful",
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                Username = user.Username,
                Email = user.Email,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };
        }

        public async Task<AuthResponse> Register(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.DeleteDate == null))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email already exists"
                };
            }

            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == request.Username.ToLower() && u.DeleteDate == null))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Username already exists"
                };
            }

            _passwordService.CreatePasswordHash(request.Password, out string passwordHash, out string salt);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                Salt = salt,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreateDate = DateTime.UtcNow
            };

            // Assign default guest role
            var guestRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Guest");

            if (guestRole == null)
            {
                // Create roles if they don't exist
                guestRole = new Role { Name = "Guest", CreateDate = DateTime.UtcNow };
                _context.Roles.Add(guestRole);

                // Create admin role as well
                var adminRole = new Role { Name = "Admin", CreateDate = DateTime.UtcNow };
                _context.Roles.Add(adminRole);

                await _context.SaveChangesAsync();
            }

            user.UserRoles.Add(new UserRole
            {
                User = user,
                RoleId = guestRole.Id
            });

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate token
            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "User registered successfully",
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                Username = user.Username,
                Email = user.Email,
                Roles = new List<string> { "Guest" }
            };
        }

        public async Task<bool> IsEmailUnique(string email)
        {
            return !await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower() && u.DeleteDate == null);
        }

        public async Task<bool> IsUsernameUnique(string username)
        {
            return !await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower() && u.DeleteDate == null);
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
