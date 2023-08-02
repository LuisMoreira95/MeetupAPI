using Microsoft.IdentityModel.Tokens;
using MMeetupAPI.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MMeetupAPI.Identity
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions jwtOptions;

        public JwtProvider(JwtOptions jwtOptions)
        {
            this.jwtOptions = jwtOptions;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("dd-MM-yyyy")),
            };

            if (!string.IsNullOrEmpty(user.Nationality)) { claims.Add(new Claim("Nationality", user.Nationality)); };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtOptions.JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddDays(this.jwtOptions.JwtExpireDays);

            var token = new JwtSecurityToken(
                this.jwtOptions.JwtIssuer,
                this.jwtOptions.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: creds
                );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);

        }
    }
}
