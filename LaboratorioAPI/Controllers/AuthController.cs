using LaboratorioAPI.Data;
using LaboratorioAPI.DTOs.Auth;
using LaboratorioAPI.Models;
using LaboratorioAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LaboratorioAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // POST: api/auth/login (anónimo)
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            {
                return Unauthorized("Username o contraseña incorrectos.");
            }

            var (token, expira) = _tokenService.GenerarToken(usuario);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Expiracion = expira,
                Username = usuario.Username,
                Rol = usuario.Rol
            });
        }

        // POST: api/auth/register (solo Admin)
        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Username == dto.Username))
            {
                return Conflict($"El Username '{dto.Username}' ya está registrado.");
            }

            var usuario = new Usuario
            {
                Username = dto.Username.Trim(),
                Email = dto.Email.Trim(),
                NombreCompleto = dto.NombreCompleto.Trim(),
                Rol = dto.Rol,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, new
            {
                usuario.Id,
                usuario.Username,
                usuario.Email,
                usuario.NombreCompleto,
                usuario.Rol
            });
        }
    }
}