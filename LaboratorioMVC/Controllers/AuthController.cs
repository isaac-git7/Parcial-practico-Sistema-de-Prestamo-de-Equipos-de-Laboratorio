using LaboratorioMVC.Models;
using LaboratorioMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace LaboratorioMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado = await _authService.LoginAsync(modelo.Username, modelo.Password);

            if (!resultado.Exito)
            {
                ModelState.AddModelError(string.Empty, resultado.Mensaje ?? "Error al iniciar sesión.");
                modelo.Password = string.Empty;
                return View(modelo);
            }

            HttpContext.Session.SetString("Token", resultado.Token!);
            HttpContext.Session.SetString("Rol", resultado.Rol!);
            HttpContext.Session.SetString("Username", resultado.Username!);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}