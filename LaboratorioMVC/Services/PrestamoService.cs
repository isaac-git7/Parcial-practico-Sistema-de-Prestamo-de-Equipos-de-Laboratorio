using System.Text;
using System.Text.Json;
using LaboratorioMVC.Models;

namespace LaboratorioMVC.Services
{
    public class PrestamoService : IPrestamoService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public PrestamoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<PrestamoListViewModel>> GetAllAsync(bool soloVencidos)
        {
            string url = "api/Prestamos" + (soloVencidos ? "?soloVencidos=true" : "");
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return Enumerable.Empty<PrestamoListViewModel>();

            var contenido = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<PrestamoListViewModel>>(contenido, _jsonOptions)
                   ?? Enumerable.Empty<PrestamoListViewModel>();
        }

        public async Task<(bool Exito, string? Error)> CreateAsync(PrestamoCreateViewModel model)
        {
            var payload = new
            {
                equipoId = model.EquipoId,
                usuarioId = model.UsuarioId,
                fechaVencimiento = model.FechaVencimiento.ToString("yyyy-MM-ddT00:00:00"),
                observaciones = model.Observaciones
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Prestamos", content);

            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await LeerErrorAsync(response, "No se pudo registrar el préstamo."));
        }

        public async Task<(bool Exito, string? Error)> DevolverAsync(int id)
        {
            var response = await _httpClient.PutAsync($"api/Prestamos/{id}/devolucion", new StringContent(string.Empty));

            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await LeerErrorAsync(response, "No se pudo registrar la devolución."));
        }

        public async Task<IEnumerable<UsuarioItemViewModel>> GetUsuariosAsync()
        {
            var response = await _httpClient.GetAsync("api/Usuarios");
            if (!response.IsSuccessStatusCode) return Enumerable.Empty<UsuarioItemViewModel>();

            var contenido = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<UsuarioItemViewModel>>(contenido, _jsonOptions)
                   ?? Enumerable.Empty<UsuarioItemViewModel>();
        }

        // La API devuelve texto plano en 400 y 409; si llega un JSON de validación, usamos el mensaje por defecto.
        private static async Task<string> LeerErrorAsync(HttpResponseMessage response, string mensajePorDefecto)
        {
            var cuerpo = (await response.Content.ReadAsStringAsync()).Trim();
            if (string.IsNullOrWhiteSpace(cuerpo) || cuerpo.StartsWith("{")) return mensajePorDefecto;
            return cuerpo.Trim('"');
        }
    }
}