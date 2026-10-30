using System.Net.Http.Headers;

namespace LaboratorioMVC.Services
{
    // Agrega el token de la sesión a cada solicitud hacia la API,
    // sin tocar DefaultRequestHeaders de un cliente compartido.
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _accessor;

        public AuthTokenHandler(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _accessor.HttpContext?.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}