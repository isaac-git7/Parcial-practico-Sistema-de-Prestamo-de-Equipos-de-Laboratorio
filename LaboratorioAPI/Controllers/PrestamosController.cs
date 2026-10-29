using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LaboratorioAPI.Data;
using LaboratorioAPI.DTOs.Prestamos;
using LaboratorioAPI.Models;

namespace LaboratorioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PrestamosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PrestamosController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // El Id del usuario autenticado siempre sale del token, nunca de un parámetro.
        private int? ObtenerUsuarioIdDelToken()
        {
            var valor = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(valor, out var id) ? id : null;
        }

        // GET: api/prestamos?soloVencidos=true
        // Admin y Docente ven todos; el Estudiante solo los propios.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrestamoDto>>> GetPrestamos(
            [FromQuery] bool? soloActivos,
            [FromQuery] bool? soloVencidos)
        {
            var query = _context.Prestamos
                .Include(p => p.Equipo)
                .Include(p => p.Usuario)
                .AsQueryable();

            if (User.IsInRole("Estudiante"))
            {
                var usuarioId = ObtenerUsuarioIdDelToken();
                if (usuarioId == null) return Unauthorized();
                query = query.Where(p => p.UsuarioId == usuarioId.Value);
            }

            if (soloActivos == true)
            {
                query = query.Where(p => p.Estado == "Activo");
            }

            if (soloVencidos == true)
            {
                var hoy = DateTime.Today;
                query = query.Where(p => p.FechaDevolucion == null && p.FechaVencimiento < hoy);
            }

            var prestamos = await query.OrderByDescending(p => p.FechaPrestamo).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<PrestamoDto>>(prestamos));
        }

        // GET: api/prestamos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PrestamoDto>> GetPrestamo(int id)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Equipo)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prestamo == null)
            {
                return NotFound($"No se encontró el préstamo con ID {id}.");
            }

            // Un Estudiante no puede consultar préstamos de otros usuarios.
            if (User.IsInRole("Estudiante") && prestamo.UsuarioId != ObtenerUsuarioIdDelToken())
            {
                return Forbid();
            }

            return Ok(_mapper.Map<PrestamoDto>(prestamo));
        }

        // POST: api/prestamos (Admin, Docente)
        [HttpPost]
        [Authorize(Roles = "Admin,Docente")]
        public async Task<ActionResult<PrestamoDto>> CrearPrestamo([FromBody] PrestamoCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DateTime fechaActual = DateTime.Now;

            // REGLA: la fecha de vencimiento no puede ser anterior a hoy
            if (dto.FechaVencimiento.Date < fechaActual.Date)
            {
                return BadRequest("La fecha de vencimiento no puede ser una fecha pasada.");
            }

            // REGLA: máximo 7 días calendario
            TimeSpan diferenciaDias = dto.FechaVencimiento.Date - fechaActual.Date;
            if (diferenciaDias.TotalDays > 7)
            {
                return BadRequest("Regla institucional: El tiempo de préstamo no puede superar los 7 días calendario.");
            }

            // Verificar la existencia del usuario solicitante
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == dto.UsuarioId);
            if (!usuarioExiste)
            {
                return BadRequest($"El usuario con ID {dto.UsuarioId} no existe en el sistema.");
            }

            // Verificar existencia y disponibilidad del equipo
            var equipo = await _context.Equipos.FindAsync(dto.EquipoId);
            if (equipo == null)
            {
                return NotFound($"El equipo con ID {dto.EquipoId} no fue encontrado.");
            }

            if (!string.Equals(equipo.Estado, "Disponible", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict($"Conflicto operativo: El equipo '{equipo.Nombre}' no está disponible para préstamo. Estado actual: '{equipo.Estado}'.");
            }

            // Transacción atómica: un solo SaveChanges para el equipo y el préstamo
            equipo.Estado = "Prestado";

            var prestamo = new Prestamo
            {
                EquipoId = dto.EquipoId,
                UsuarioId = dto.UsuarioId,
                FechaPrestamo = fechaActual,
                FechaVencimiento = dto.FechaVencimiento,
                FechaDevolucion = null,
                Estado = "Activo",
                Observaciones = dto.Observaciones?.Trim()
            };

            _context.Prestamos.Add(prestamo);
            await _context.SaveChangesAsync();

            await _context.Entry(prestamo).Reference(p => p.Equipo).LoadAsync();
            await _context.Entry(prestamo).Reference(p => p.Usuario).LoadAsync();

            var resultadoDto = _mapper.Map<PrestamoDto>(prestamo);
            return CreatedAtAction(nameof(GetPrestamo), new { id = prestamo.Id }, resultadoDto);
        }

        // PUT: api/prestamos/5/devolucion (Admin, Docente)
        [HttpPut("{id}/devolucion")]
        [Authorize(Roles = "Admin,Docente")]
        public async Task<IActionResult> DevolverEquipo(int id)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Equipo)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prestamo == null)
            {
                return NotFound($"El registro de préstamo con ID {id} no existe.");
            }

            if (prestamo.Estado == "Devuelto")
            {
                return Conflict("El préstamo indicado ya ha sido registrado como devuelto anteriormente.");
            }

            prestamo.FechaDevolucion = DateTime.Now;
            prestamo.Estado = "Devuelto";

            if (prestamo.Equipo != null)
            {
                prestamo.Equipo.Estado = "Disponible";
            }

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Equipo devuelto correctamente al laboratorio y marcado como Disponible." });
        }
    }
}