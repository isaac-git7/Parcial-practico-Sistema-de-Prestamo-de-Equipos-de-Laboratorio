using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LaboratorioMVC.Models;
using LaboratorioMVC.Services;

namespace LaboratorioMVC.Controllers
{
    public class EquiposController : Controller
    {
        private readonly IEquipoService _equipoService;
        private readonly ICategoriaService _categoriaService;

        public EquiposController(IEquipoService equipoService, ICategoriaService categoriaService)
        {
            _equipoService = equipoService;
            _categoriaService = categoriaService;
        }

        // GET: Equipos?buscar=laptop&categoriaId=1&estado=Disponible&pagina=1
        public async Task<IActionResult> Index(string? buscar, int? categoriaId, string? estado, int pagina = 1)
        {
            const int tamanioPagina = 10;
            if (pagina < 1) pagina = 1;

            if (HttpContext.Session.GetString("Rol") == "Estudiante")
            {
                estado = "Disponible";
            }

            // Obtener los registros filtrados desde la API
            var todosLosEquipos = (await _equipoService.GetAllAsync(buscar, categoriaId, estado)).ToList();
            var categorias = await _categoriaService.GetAllAsync();

            int totalRegistros = todosLosEquipos.Count;
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanioPagina);
            if (totalPaginas == 0) totalPaginas = 1;
            if (pagina > totalPaginas) pagina = totalPaginas;

            // Paginación en memoria de 10 elementos
            var equiposPaginados = todosLosEquipos
                .Skip((pagina - 1) * tamanioPagina)
                .Take(tamanioPagina)
                .ToList();

            var viewModel = new EquipoFiltroViewModel
            {
                Buscar = buscar,
                CategoriaId = categoriaId,
                Estado = estado,
                Equipos = equiposPaginados,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas,
                TotalRegistros = totalRegistros,
                TamanioPagina = tamanioPagina,
                Categorias = categorias.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre,
                    Selected = (categoriaId == c.Id)
                })
            };

            return View(viewModel);
        }

        // GET: Equipos/Create
        public async Task<IActionResult> Create()
        {
            var model = new EquipoFormViewModel
            {
                CategoriasDisponibles = await ObtenerCategoriasDropdownAsync()
            };
            return View(model);
        }

        // POST: Equipos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EquipoFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CategoriasDisponibles = await ObtenerCategoriasDropdownAsync();
                return View(model);
            }

            var (exito, error, _) = await _equipoService.CreateAsync(model);
            if (exito)
            {
                TempData["MensajeExito"] = "Equipo registrado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, error ?? "Error al registrar el equipo.");
            model.CategoriasDisponibles = await ObtenerCategoriasDropdownAsync();
            return View(model);
        }

        // GET: Equipos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _equipoService.GetByIdAsync(id);
            if (model == null) return NotFound();

            model.CategoriasDisponibles = await ObtenerCategoriasDropdownAsync();
            return View(model);
        }

        // POST: Equipos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EquipoFormViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                model.CategoriasDisponibles = await ObtenerCategoriasDropdownAsync();
                return View(model);
            }

            var (exito, error, esConflicto) = await _equipoService.UpdateAsync(id, model);

            if (exito)
            {
                TempData["MensajeExito"] = "Equipo actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            if (esConflicto)
            {
                // Concurrencia optimista detectada
                ModelState.AddModelError(string.Empty,
                    "⚠️ Conflicto de Concurrencia: Este equipo fue modificado por otro usuario mientras lo editabas. Los datos actuales no fueron guardados para evitar sobrescribir información. Por favor cancela y vuelve a abrir el registro.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, error ?? "Error al actualizar el equipo.");
            }

            model.CategoriasDisponibles = await ObtenerCategoriasDropdownAsync();
            return View(model);
        }

        // POST: Equipos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (exito, error) = await _equipoService.DeleteAsync(id);
            if (exito)
            {
                TempData["MensajeExito"] = "Equipo eliminado correctamente.";
            }
            else
            {
                TempData["MensajeError"] = error;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCategoriasDropdownAsync()
        {
            var categorias = await _categoriaService.GetAllAsync();
            return categorias.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nombre
            });
        }
    }
}