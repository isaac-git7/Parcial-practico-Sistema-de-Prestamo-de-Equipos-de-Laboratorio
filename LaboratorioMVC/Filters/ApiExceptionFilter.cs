using System.Net;
using LaboratorioMVC.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LaboratorioMVC.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ApiException api)
            {
                if (api.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // 401: limpiar la sesión y volver al login con aviso
                    context.HttpContext.Session.Clear();
                    context.Result = new RedirectToActionResult("Login", "Auth", new { expirada = true });
                }
                else if (api.StatusCode == HttpStatusCode.Forbidden)
                {
                    // 403: vista de acceso denegado
                    context.Result = new RedirectToActionResult("AccesoDenegado", "Error", null);
                }
                else
                {
                    // 5xx: mensaje genérico (el detalle solo va al log)
                    _logger.LogError(context.Exception, "Error del servidor de la API");
                    context.Result = new RedirectToActionResult("Servidor", "Error", null);
                }

                context.ExceptionHandled = true;
                return;
            }

            // API apagada o sin respuesta
            if (context.Exception is HttpRequestException || context.Exception is TaskCanceledException)
            {
                _logger.LogError(context.Exception, "No se pudo comunicar con la API");
                context.Result = new RedirectToActionResult("Servidor", "Error", null);
                context.ExceptionHandled = true;
            }
        }
    }
}