using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LaboratorioAPI.Data;
using LaboratorioAPI.DTOs.Prestamos;
using LaboratorioAPI.Models;

namespace LaboratorioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrestamosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PrestamosController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/prestamos
        // Lista todos los préstamos registrados con opción de filtrar solo los pendientes/activos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrestamoDto>>> GetPrestamos([FromQuery] bool? soloActivos)
        {
            var query = _context.Prestamos
                .Include(p => p.Equipo)
                .Include(p => p.Usuario)
                .AsQueryable();

            if (soloActivos.HasValue && soloActivos.Value)
            {
                query = query.Where(p => p.Estado == "Activo");
            }

            var prestamos = await query.OrderByDescending(p => p.FechaPrestamo).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<PrestamoDto>>(prestamos));
        }

        // GET: api/prestamos/5
        // Consulta el detalle de un préstamo por su ID
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

            return Ok(_mapper.Map<PrestamoDto>(prestamo));
        }

        // POST: api/prestamos
        // Registra un nuevo préstamo aplicando las reglas de negocio institucionales
        [HttpPost]
        public async Task<ActionResult<PrestamoDto>> CrearPrestamo([FromBody] PrestamoCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DateTime fechaActual = DateTime.Now;

            // REGLA 1: La fecha de vencimiento no puede ser anterior a la fecha de préstamo
            if (dto.FechaVencimiento.Date < fechaActual.Date)
            {
                return BadRequest("La fecha de vencimiento no puede ser una fecha pasada.");
            }

            // REGLA 2: Límite máximo de 7 días calendario
            TimeSpan diferenciaDias = dto.FechaVencimiento.Date - fechaActual.Date;
            if (diferenciaDias.TotalDays > 7)
            {
                return BadRequest("Regla institucional: El tiempo de préstamo no puede superar los 7 días calendario.");
            }

            // REGLA 3: Verificar la existencia del usuario solicitante
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == dto.UsuarioId);
            if (!usuarioExiste)
            {
                return BadRequest($"El usuario con ID {dto.UsuarioId} no existe en el sistema.");
            }

            // REGLA 4: Verificar la existencia y disponibilidad del equipo
            var equipo = await _context.Equipos.FindAsync(dto.EquipoId);
            if (equipo == null)
            {
                return NotFound($"El equipo con ID {dto.EquipoId} no fue encontrado.");
            }

            // Si el equipo no está Disponible (está 'Prestado', 'EnMantenimiento' o 'DeBaja'), retornamos 409 Conflict
            if (!string.Equals(equipo.Estado, "Disponible", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict($"Conflicto operativo: El equipo '{equipo.Nombre}' no está disponible para préstamo. Estado actual: '{equipo.Estado}'.");
            }

            // TRANSACCIONALIDAD ATÓMICA:
            // Preparamos dos cambios en memoria:
            // 1. Cambiamos el estado del equipo físico a 'Prestado'
            equipo.Estado = "Prestado";

            // 2. Construimos el nuevo registro de préstamo
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

            // Se ejecutan ambas operaciones en una sola transacción hacia SQL Server.
            // Si una falla, ninguna se aplica.
            await _context.SaveChangesAsync();

            // Cargamos las entidades relacionadas para armar el DTO de respuesta
            await _context.Entry(prestamo).Reference(p => p.Equipo).LoadAsync();
            await _context.Entry(prestamo).Reference(p => p.Usuario).LoadAsync();

            var resultadoDto = _mapper.Map<PrestamoDto>(prestamo);

            return CreatedAtAction(nameof(GetPrestamo), new { id = prestamo.Id }, resultadoDto);
        }

        // POST: api/prestamos/5/devolver
        // Cierra un préstamo activo y libera el equipo para que vuelva a estar Disponible
        [HttpPost("{id}/devolver")]
        public async Task<IActionResult> DevolverEquipo(int id)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Equipo)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prestamo == null)
            {
                return NotFound($"El registro de préstamo con ID {id} no existe.");
            }

            // Si el préstamo ya fue devuelto previamente, no tiene sentido volver a cerrarlo
            if (prestamo.Estado == "Devuelto")
            {
                return Conflict("El préstamo indicado ya ha sido registrado como devuelto anteriormente.");
            }

            // Actualizamos los datos de devolución
            prestamo.FechaDevolucion = DateTime.Now;
            prestamo.Estado = "Devuelto";

            // Si el equipo sigue asociado, lo marcamos como Disponible nuevamente
            if (prestamo.Equipo != null)
            {
                prestamo.Equipo.Estado = "Disponible";
            }

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Equipo devuelto correctamente al laboratorio y marcado como Disponible." });
        }
    }
}