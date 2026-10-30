using LaboratorioAPI.Data;
using LaboratorioAPI.DTOs.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LaboratorioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Docente")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/usuarios
        // Lista de usuarios para elegir a quién se presta un equipo (nunca expone el hash).
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioListDto>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .AsNoTracking()
                .OrderBy(u => u.Username)
                .Select(u => new UsuarioListDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    NombreCompleto = u.NombreCompleto,
                    Rol = u.Rol
                })
                .ToListAsync();

            return Ok(usuarios);
        }
    }
}