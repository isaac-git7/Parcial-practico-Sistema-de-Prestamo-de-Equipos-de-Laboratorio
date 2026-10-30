using System.Security.Claims;
using System.Text;
using LaboratorioAPI.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace LaboratorioAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public (string Token, DateTime Expira) GenerarToken(Usuario usuario)
        {
            var clave = _config["Jwt:Key"]
                ?? throw new InvalidOperationException("Falta la clave Jwt:Key en User Secrets.");

            var minutos = int.TryParse(_config["Jwt:ExpireMinutes"], out var m) ? m : 60;
            var expira = DateTime.UtcNow.AddMinutes(minutos);

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Username),
                    new Claim(ClaimTypes.Role, usuario.Rol)
                }),
                Expires = expira,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),
                    SecurityAlgorithms.HmacSha256)
            };

            var token = new JsonWebTokenHandler().CreateToken(descriptor);
            return (token, expira);
        }
    }
}