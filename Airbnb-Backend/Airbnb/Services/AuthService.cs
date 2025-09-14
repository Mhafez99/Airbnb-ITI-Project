using Airbnb.Db;
using Airbnb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Airbnb.Services
{
    public class AuthService
    {
        private readonly BookingContext _context;
        private readonly IConfiguration _config;

        public AuthService(BookingContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ⬅️ login
        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;

            bool valid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!valid) return null;

            return user;
        }

        // ⬅️ signup
        public async Task<User> SignupAsync(User newUser)
        {
            if (string.IsNullOrEmpty(newUser.Username) ||
                string.IsNullOrEmpty(newUser.Password) ||
                string.IsNullOrEmpty(newUser.Fullname))
            {
                throw new Exception("Missing required signup information");
            }

            if (await _context.Users.AnyAsync(u => u.Username == newUser.Username))
                throw new Exception("Username already taken");

            newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        // ⬅️ Generate JWT Token (بدل Cryptr)
        public string GetLoginToken(User user)
        {
            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("fullname", user.Fullname),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "Secret-Puk-1234"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
                throw new Exception("User not found");

            existingUser.Fullname = user.Fullname ?? existingUser.Fullname;
            existingUser.Username = user.Username ?? existingUser.Username;
            existingUser.Password = user.Password ?? existingUser.Password;
            existingUser.ImgUrl = user.ImgUrl ?? existingUser.ImgUrl;
            existingUser.UserMsg = user.UserMsg;
            existingUser.HostMsg = user.HostMsg;

            await _context.SaveChangesAsync();

            return existingUser;
        }

        // ⬅️ Validate JWT
        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "Secret-Puk-1234");

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}

