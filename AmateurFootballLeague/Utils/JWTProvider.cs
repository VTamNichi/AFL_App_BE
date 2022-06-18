using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using AmateurFootballLeague.Models;

namespace AmateurFootballLeague.Utils
{
    public interface IJWTProvider
    {
        Task<string> GenerationToken(User user);
    }

    public class JWTProvider : IJWTProvider
    {
        private readonly IConfiguration _configuration;
        public JWTProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task<string> GenerationToken(User user)
        {
            return Task.Run(() =>
            {
                List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id + ""),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.Username!),
                new Claim(ClaimTypes.Role, user.Role!.RoleName + "")
            };
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(8),
                    signingCredentials: cred);

                string jwt = new JwtSecurityTokenHandler().WriteToken(token);

                return jwt;
            });
        }
    }
}
