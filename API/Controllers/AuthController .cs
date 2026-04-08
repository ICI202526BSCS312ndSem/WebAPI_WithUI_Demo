using API.Settings;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace YourProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly string _secretKey = GlobalSettings.SECRET_KEY;

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var permissions = new List<string>();

            if (request.Email == "view@test.com" && request.Password == "password123")
            {
                permissions.AddRange(new[] { "view_product" });
            }
            else if (request.Email == "viewadd@test.com" && request.Password == "password123")
            {
                permissions.AddRange(new[] { "view_product", "add_product" });
            }
            else if (request.Email == "viewaddedit@test.com" && request.Password == "password123")
            {
                permissions.AddRange(new[] { "view_product", "add_product", "edit_product" });
            }
            else if (request.Email == "admin@test.com" && request.Password == "password123")
            {
                permissions.AddRange(new[] { "view_product", "add_product", "edit_product", "delete_product" });
            }
            else
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var token = GenerateJwtToken(request.Email, permissions);

            return Ok(new
            {
                token = token,
                email = request.Email,
                permissions = permissions
            });
        }

        private string GenerateJwtToken(string email, List<string> permissions)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            // Add the email as a Name claim
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, email) };

            // Add each permission as a separate "permission" claim for the API Policy check
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
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
            return tokenHandler.WriteToken(token);
        }
    }
}