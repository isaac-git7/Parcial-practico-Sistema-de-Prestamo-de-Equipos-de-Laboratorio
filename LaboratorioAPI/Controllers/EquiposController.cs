using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LaboratorioAPI.Data;
using LaboratorioAPI.DTOs.Equipos;
using LaboratorioAPI.Models;

namespace LaboratorioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquiposController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public EquiposController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/equipos?buscar=laptop&categoriaId=1&estado=Disponible
        // Lista los equipos permitiendo combinar filtros opcionales de búsqueda
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipoListDto>>> GetEquipos(
            [FromQuery] string? buscar,
            [FromQuery] int? categoriaId,
            [FromQuery] string? estado)
        {
            // Iniciamos la consulta incluyendo los datos de la Categoria relacionada (Eager Loading)
            var query = _context.Equipos
                .Include(e => e.Categoria)
                .AsQueryable();

            // Filtro 1: Buscador por texto (coincidencia en nombre o código de inventario)
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                string termino = buscar.Trim().ToLower();
                query = query.Where(e => e.Nombre.ToLower().Contains(termino)
                                      || e.CodigoInventario.ToLower().Contains(termino));
            }

            // Filtro 2: Filtrado por identificador de categoría
            if (categoriaId.HasValue && categoriaId.Value > 0)
            {
                query = query.Where(e => e.CategoriaId == categoriaId.Value);
            }

            // Filtro 3: Filtrado por estado operativo del equipo
            if (!string.IsNullOrWhiteSpace(estado))
            {
                query = query.Where(e => e.Estado.ToLower() == estado.Trim().ToLower());
            }

            var equipos = await query.ToListAsync();

            // Convertimos la lista de entidades al DTO ligero para la grilla
            var equiposDto = _mapper.Map<IEnumerable<EquipoListDto>>(equipos);

            return Ok(equiposDto);
        }

        // GET: api/equipos/5
        // Obtiene la ficha técnica completa de un equipo, incluyendo su RowVersion en Base64
        [HttpGet("{id}")]
        public async Task<ActionResult<EquipoDetalleDto>> GetEquipo(int id)
        {
            var equipo = await _context.Equipos
                .Include(e => e.Categoria)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipo == null)
            {
                return NotFound($"El equipo con ID {id} no existe en el sistema.");
            }

            var dto = _mapper.Map<EquipoDetalleDto>(equipo);
            return Ok(dto);
        }

        // POST: api/equipos
        // Registra un nuevo equipo físico en el inventario
        [HttpPost]
        public async Task<ActionResult<EquipoDetalleDto>> CrearEquipo([FromBody] EquipoCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Validar que la categoría seleccionada exista en la base de datos
            bool categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
            if (!categoriaExiste)
            {
                return BadRequest($"La categoría con ID {dto.CategoriaId} no existe.");
            }

            // 2. Regla de negocio: El código de inventario debe ser único en toda la institución
            bool codigoRepetido = await _context.Equipos
                .AnyAsync(e => e.CodigoInventario.ToLower() == dto.CodigoInventario.Trim().ToLower());

            if (codigoRepetido)
            {
                return Conflict($"Ya existe un equipo registrado con el código '{dto.CodigoInventario}'.");
            }

            // Mapeamos el DTO a la entidad Equipo
            var equipo = _mapper.Map<Equipo>(dto);
            equipo.CodigoInventario = equipo.CodigoInventario.Trim().ToUpper();
            equipo.Nombre = equipo.Nombre.Trim();

            _context.Equipos.Add(equipo);
            await _context.SaveChangesAsync();

            // Consultamos el registro recién creado para cargar la categoría asociada
            await _context.Entry(equipo).Reference(e => e.Categoria).LoadAsync();

            var resultadoDto = _mapper.Map<EquipoDetalleDto>(equipo);

            return CreatedAtAction(nameof(GetEquipo), new { id = equipo.Id }, resultadoDto);
        }

        // PUT: api/equipos/5
        // Actualiza los datos de un equipo controlando concurrencia con RowVersion
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarEquipo(int id, [FromBody] EquipoUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("El identificador de la URL no coincide con el cuerpo enviado.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Buscamos el equipo en la base de datos
            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null)
            {
                return NotFound($"No se encontró el equipo con ID {id}.");
            }

            // Validamos que la categoría destino exista
            bool categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
            if (!categoriaExiste)
            {
                return BadRequest($"La categoría seleccionada no es válida.");
            }

            // CONCURRENCIA OPTIMISTA:
            // Le indicamos a Entity Framework cuál era la versión del registro que el usuario leyó.
            // Si otra persona modificó el equipo antes, la versión en BD será distinta y saltará la excepción.
            try
            {
                byte[] versionCliente = Convert.FromBase64String(dto.RowVersion);
                _context.Entry(equipo).Property(e => e.RowVersion).OriginalValue = versionCliente;
            }
            catch (FormatException)
            {
                return BadRequest("El formato del sello de versión (RowVersion) no es válido.");
            }

            // Actualizamos los campos editables
            equipo.Nombre = dto.Nombre.Trim();
            equipo.Marca = dto.Marca?.Trim();
            equipo.Modelo = dto.Modelo?.Trim();
            equipo.Serie = dto.Serie?.Trim();
            equipo.CategoriaId = dto.CategoriaId;
            equipo.Estado = dto.Estado.Trim();

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); // 204: Actualización exitosa
            }
            catch (DbUpdateConcurrencyException)
            {
                // Choque de concurrencia detectado: retornamos 409 Conflict
                return StatusCode(StatusCodes.Status409Conflict,
                    "Conflicto de concurrencia: El equipo fue modificado por otro usuario mientras lo editabas. Recarga la información antes de guardar.");
            }
        }

        // DELETE: api/equipos/5
        // Da de baja un equipo verificando que no tenga historial de préstamos
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarEquipo(int id)
        {
            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null)
            {
                return NotFound($"El equipo con ID {id} no fue encontrado.");
            }

            // Regla de negocio: No se puede eliminar si ya ha tenido préstamos registrados
            bool tienePrestamos = await _context.Prestamos.AnyAsync(p => p.EquipoId == id);
            if (tienePrestamos)
            {
                return Conflict("No se puede eliminar este equipo porque cuenta con un historial de préstamos asociado.");
            }

            _context.Equipos.Remove(equipo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}