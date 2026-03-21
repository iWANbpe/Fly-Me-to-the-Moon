using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fly_Me_to_the_Moon.Services
{
    public class AuthService
    {
        private readonly SpaceFlightContext _context;
        private readonly IConfiguration _config;

        public AuthService(SpaceFlightContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string> LoginAdmin(LoginDto dto)
        {
            var admin = await _context.Admin
                .FirstOrDefaultAsync(a => a.AdminName == dto.Username);

            if (admin == null || !BCrypt.Net.BCrypt.Verify(dto.Password, admin.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid admin credentials.");
            }

            return CreateToken(admin.AdminId.ToString(), admin.AdminName, "Admin");
        }

        public async Task<string> LoginPassenger(LoginDto dto)
        {
            var passenger = await _context.Passenger
                .FirstOrDefaultAsync(p => p.Email == dto.Username);

            if (passenger == null || !BCrypt.Net.BCrypt.Verify(dto.Password, passenger.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid passenger credentials.");
            }

            return CreateToken(passenger.PassengerId.ToString(), passenger.Email, "Passenger");
        }

        private string CreateToken(string id, string name, string role)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Role, role)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
