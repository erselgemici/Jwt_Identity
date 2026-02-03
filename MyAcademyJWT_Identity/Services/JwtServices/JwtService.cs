using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MyAcademyJWT_Identity.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyAcademyJWT_Identity.Services.JwtServices
{
    public class JwtService(UserManager<AppUser> _userManager, IConfiguration _configuration) : IJwtService
    {
        public async Task<string> GenerateTokenAsync(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            SymmetricSecurityKey symmetricSecurityKey = new(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            JwtSecurityToken jwtSecurityToken = new
                (
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: new SigningCredentials(symmetricSecurityKey,SecurityAlgorithms.HmacSha256)
                );

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
            
        }
    }
}
