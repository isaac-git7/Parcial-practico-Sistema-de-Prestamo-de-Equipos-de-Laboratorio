using System.Net;

namespace LaboratorioMVC.Services
{
    // Convierte 401, 403 y 5xx de la API en una excepción que atiende un filtro global.
    // Los demás códigos (200, 400, 404, 409) siguen su camino normal.
    public class ApiErrorHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            var codigo = response.StatusCode;

            if (codigo == HttpStatusCode.Unauthorized ||
                codigo == HttpStatusCode.Forbidden ||
                (int)codigo >= 500)
            {
                response.Dispose();
                throw new ApiException(codigo);
            }

            return response;
        }
    }
}