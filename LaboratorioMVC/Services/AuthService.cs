using System.Net;
using System.Text.Json;

namespace LaboratorioMVC.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;

        public AuthService(HttpClient http)
        {
            _http = http;
        }

        public async Task<LoginResultado> LoginAsync(string username, string password)
        {
            try
            {
                var respuesta = await _http.PostAsJsonAsync("api/auth/login", new { username, password });

                if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return new LoginResultado(false, "Username o contraseña incorrectos.", null, null, null);
                }

                if (!respuesta.IsSuccessStatusCode)
                {
                    return new LoginResultado(false, "No se pudo iniciar sesión. Intente de nuevo más tarde.", null, null, null);
                }

                using var doc = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync());
                var raiz = doc.RootElement;

                return new LoginResultado(
                    true,
                    null,
                    raiz.GetProperty("token").GetString(),
                    raiz.GetProperty("rol").GetString(),
                    raiz.GetProperty("username").GetString());
            }
            catch (HttpRequestException)
            {
                return new LoginResultado(false, "No se pudo conectar con el servidor. Verifique que la API esté en ejecución.", null, null, null);
            }
        }
    }
}