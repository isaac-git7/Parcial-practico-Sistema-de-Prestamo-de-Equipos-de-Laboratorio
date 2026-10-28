using Microsoft.AspNetCore.Mvc;
using LaboratorioMVC.Models;
using LaboratorioMVC.Services;

namespace LaboratorioMVC.Controllers
{
    public class ReportesController : Controller
    {
        private readonly IReporteService _reporteService;

        public ReportesController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        // GET: Reportes
        public async Task<IActionResult> Index()
        {
            var modelo = await _reporteService.ObtenerTableroCompletoAsync();
            return View(modelo);
        }

        // GET: Reportes/Historial?termino=EST-001
        public async Task<IActionResult> Historial(string? termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
            {
                return View(new HistorialUsuarioViewModel());
            }

            var modelo = await _reporteService.ObtenerHistorialPorUsuarioAsync(termino);
            return View(modelo);
        }
    }
}