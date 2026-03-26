using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AuthController : ControllerBase
    {
        private readonly string _secretKey = "ANOBAMAKULITKABA_ANOBAMAKULITKABA";

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var permissions = new List<string>(); // New
            if (request.Email == "viewadmin@test.com" && request.Password == "password123")
            {
                permissions.AddRange(new[] { "view_product" }); // View product
            }

            else if (request.Email == "addadmin@test.com" && request.Password == "password123")
            {
                permissions.AddRange(new[] { "view_product", "add_product" }); // View product and Add product
            }

            else if (request.Email == "editadmin@test.com" && request.Password == "password123")
            {
                permissions.AddRange(new[] { "view_product", "add_product", "edit_product" }); // View product, Add product and Edit product
            }

            else if (request.Email == "deleteadmin@test.com" && request.Password == "password123")
            {
                permissions.AddRange(new[] { "view_product", "add_product", "edit_product", "delete_product" }); // Add all
            }


            else
                return Unauthorized(new { message = "Invalid email or password." });

            var token = GenerateJwtToken(request.Email, permissions);
            return Ok(new
            {
                token = token,
                email = request.Email,
            });
        }

        private String GenerateJwtToken(string email, List<string> permissions)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var claims = new List<Claim>();

            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(claims), // Add here Claim
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenhandler.CreateToken(tokenDescriptor);
            return tokenhandler.WriteToken(token);
        }
    }
}
