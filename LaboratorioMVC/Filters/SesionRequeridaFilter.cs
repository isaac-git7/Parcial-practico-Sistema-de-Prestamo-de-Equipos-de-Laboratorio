using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LaboratorioMVC.Filters
{
    // Si no hay token en la sesión, manda al login (excepto en Auth, Error y Home).
    public class SesionRequeridaFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controlador = context.RouteData.Values["controller"]?.ToString();
            if (controlador is "Auth" or "Error" or "Home") return;

            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString("Token")))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}