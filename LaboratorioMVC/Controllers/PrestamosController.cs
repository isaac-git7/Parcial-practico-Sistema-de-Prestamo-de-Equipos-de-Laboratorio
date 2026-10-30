using LaboratorioMVC.Models;
using LaboratorioMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LaboratorioMVC.Controllers
{
    public class PrestamosController : Controller
    {
        private readonly IPrestamoService _prestamoService;
        private readonly IEquipoService _equipoService;

        public PrestamosController(IPrestamoService prestamoService, IEquipoService equipoService)
        {
            _prestamoService = prestamoService;
            _equipoService = equipoService;
        }

        private bool PuedeGestionar =>
            HttpContext.Session.GetString("Rol") is "Admin" or "Docente";

        // GET: Prestamos?soloVencidos=true
        // Admin y Docente ven todos; el Estudiante solo los propios (lo decide la API con el token).
        public async Task<IActionResult> Index(bool soloVencidos = false)
        {
            var prestamos = await _prestamoService.GetAllAsync(soloVencidos);
            ViewBag.SoloVencidos = soloVencidos;
            return View(prestamos);
        }

        // GET: Prestamos/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!PuedeGestionar) return RedirectToAction("AccesoDenegado", "Error");

            var model = new PrestamoCreateViewModel { FechaVencimiento = DateTime.Today.AddDays(3) };
            await CargarListasAsync(model);
            return View(model);
        }

        // POST: Prestamos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrestamoCreateViewModel model)
        {
            if (!PuedeGestionar) return RedirectToAction("AccesoDenegado", "Error");

            if (!ModelState.IsValid)
            {
                await CargarListasAsync(model);
                return View(model);
            }

            var (exito, error) = await _prestamoService.CreateAsync(model);
            if (exito)
            {
                TempData["MensajeExito"] = "Préstamo registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            // 400 / 409: el mensaje de la API se muestra dentro del formulario, sin perder lo escrito
            ModelState.AddModelError(string.Empty, error ?? "No se pudo registrar el préstamo.");
            await CargarListasAsync(model);
            return View(model);
        }

        // POST: Prestamos/Devolver/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Devolver(int id)
        {
            if (!PuedeGestionar) return RedirectToAction("AccesoDenegado", "Error");

            var (exito, error) = await _prestamoService.DevolverAsync(id);
            if (exito)
            {
                TempData["MensajeExito"] = "Devolución registrada. El equipo quedó Disponible.";
            }
            else
            {
                TempData["MensajeError"] = error;
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListasAsync(PrestamoCreateViewModel model)
        {
            var equipos = await _equipoService.GetAllAsync(null, null, "Disponible");
            model.EquiposDisponibles = equipos.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = $"{e.CodigoInventario} - {e.Nombre}"
            });

            var usuarios = await _prestamoService.GetUsuariosAsync();
            model.Usuarios = usuarios.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = $"{u.Username} - {u.NombreCompleto}"
            });
        }
    }
}