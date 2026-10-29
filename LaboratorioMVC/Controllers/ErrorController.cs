using Microsoft.AspNetCore.Mvc;

namespace LaboratorioMVC.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult AccesoDenegado() => View();
        public IActionResult Servidor() => View();
    }
}