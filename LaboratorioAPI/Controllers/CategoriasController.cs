using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LaboratorioAPI.Data;
using LaboratorioAPI.DTOs.Categorias;
using LaboratorioAPI.Models;

namespace LaboratorioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        // Recibimos la base de datos y AutoMapper por inyeccion de dependencias
        public CategoriasController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/categorias
        // Obtiene todas las categorias registradas en la base de datos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
        {
            var categorias = await _context.Categorias.ToListAsync();

            // Transformamos las entidades directas a DTOs usando AutoMapper
            var categoriasDto = _mapper.Map<IEnumerable<CategoriaDto>>(categorias);

            return Ok(categoriasDto);
        }

        // GET: api/categorias/5
        // Busca una categoria especifica por su identificador unico (ID)
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound($"La categoría con ID {id} no fue encontrada.");
            }

            return Ok(_mapper.Map<CategoriaDto>(categoria));
        }

        // POST: api/categorias
        // Registra una nueva categoria a partir de los datos enviados en el formulario
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoriaDto>> CrearCategoria([FromBody] CategoriaCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Regla de unicidad: Verificar si ya existe una categoria con ese mismo nombre
            bool yaExiste = await _context.Categorias
                .AnyAsync(c => c.Nombre.ToLower() == dto.Nombre.Trim().ToLower());

            if (yaExiste)
            {
                // Devolvemos 409 Conflict si el nombre esta duplicado
                return Conflict($"Ya existe una categoría registrada con el nombre '{dto.Nombre}'.");
            }

            // Convertimos el DTO a la entidad Categoria
            var categoria = _mapper.Map<Categoria>(dto);
            categoria.Nombre = categoria.Nombre.Trim();

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            // Mapeamos de vuelta a DTO para responder al cliente con el ID generado
            var resultadoDto = _mapper.Map<CategoriaDto>(categoria);

            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, resultadoDto);
        }

        // PUT: api/categorias/5
        // Modifica el nombre o descripcion de una categoria existente
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActualizarCategoria(int id, [FromBody] CategoriaCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound($"No existe ninguna categoría con ID {id}.");
            }

            // Verificamos que no intente renombrarla con el nombre de otra categoria existente
            bool nombreEnUso = await _context.Categorias
                .AnyAsync(c => c.Id != id && c.Nombre.ToLower() == dto.Nombre.Trim().ToLower());

            if (nombreEnUso)
            {
                return Conflict($"El nombre '{dto.Nombre}' ya está siendo utilizado por otra categoría.");
            }

            // Actualizamos las propiedades
            categoria.Nombre = dto.Nombre.Trim();
            categoria.Descripcion = dto.Descripcion;

            await _context.SaveChangesAsync();

            return NoContent(); // 204: Modificacion exitosa sin necesidad de enviar cuerpo
        }

        // DELETE: api/categorias/5
        // Elimina la categoria unicamente si no tiene ningun equipo asignado
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound($"No existe ninguna categoría con ID {id}.");
            }

            // Regla de integridad: No permitir borrar categorias que tengan equipos vinculados
            bool tieneEquipos = await _context.Equipos.AnyAsync(e => e.CategoriaId == id);
            if (tieneEquipos)
            {
                return Conflict("No es posible eliminar esta categoría porque tiene equipos asociados en el inventario.");
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}