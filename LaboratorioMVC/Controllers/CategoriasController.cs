using Microsoft.AspNetCore.Mvc;
using LaboratorioMVC.Models;
using LaboratorioMVC.Services;

namespace LaboratorioMVC.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // GET: Categorias
        public async Task<IActionResult> Index()
        {
            var categorias = await _categoriaService.GetAllAsync();
            return View(categorias);
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View(new CategoriaViewModel());
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (exito, error) = await _categoriaService.CreateAsync(model);
            if (exito)
            {
                TempData["MensajeExito"] = "Categoría creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, error ?? "Error al guardar la categoría.");
            return View(model);
        }

        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var categoria = await _categoriaService.GetByIdAsync(id);
            if (categoria == null) return NotFound();

            return View(categoria);
        }

        // POST: Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoriaViewModel model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var (exito, error) = await _categoriaService.UpdateAsync(id, model);
            if (exito)
            {
                TempData["MensajeExito"] = "Categoría actualizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, error ?? "Error al actualizar.");
            return View(model);
        }

        // POST: Categorias/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (exito, error) = await _categoriaService.DeleteAsync(id);
            if (exito)
            {
                TempData["MensajeExito"] = "Categoría eliminada correctamente.";
            }
            else
            {
                TempData["MensajeError"] = error;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}