using System.Net;
using System.Text;
using System.Text.Json;
using LaboratorioMVC.Models;

namespace LaboratorioMVC.Services
{
    public class EquipoService : IEquipoService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public EquipoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IEnumerable<EquipoListViewModel>> GetAllAsync(string? buscar, int? categoriaId, string? estado)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(buscar)) queryParams.Add($"buscar={Uri.EscapeDataString(buscar)}");
            if (categoriaId.HasValue && categoriaId.Value > 0) queryParams.Add($"categoriaId={categoriaId.Value}");
            if (!string.IsNullOrWhiteSpace(estado)) queryParams.Add($"estado={Uri.EscapeDataString(estado)}");

            string url = "api/Equipos" + (queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "");
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) return Enumerable.Empty<EquipoListViewModel>();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<EquipoListViewModel>>(content, _jsonOptions)
                   ?? Enumerable.Empty<EquipoListViewModel>();
        }

        public async Task<EquipoFormViewModel?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/Equipos/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<EquipoFormViewModel>(content, _jsonOptions);
        }

        public async Task<(bool Exito, string? Error, bool EsConflicto)> CreateAsync(EquipoFormViewModel model)
        {
            var payload = new
            {
                codigoInventario = model.CodigoInventario,
                nombre = model.Nombre,
                marca = model.Marca,
                modelo = model.Modelo,
                serie = model.Serie,
                categoriaId = model.CategoriaId,
                estado = model.Estado
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Equipos", content);

            if (response.IsSuccessStatusCode) return (true, null, false);

            var errorMsg = await response.Content.ReadAsStringAsync();
            bool esConflicto = response.StatusCode == HttpStatusCode.Conflict;
            return (false, string.IsNullOrWhiteSpace(errorMsg) ? "Error al registrar el equipo." : errorMsg, esConflicto);
        }

        public async Task<(bool Exito, string? Error, bool EsConflicto)> UpdateAsync(int id, EquipoFormViewModel model)
        {
            var payload = new
            {
                id = model.Id,
                nombre = model.Nombre,
                marca = model.Marca,
                modelo = model.Modelo,
                serie = model.Serie,
                categoriaId = model.CategoriaId,
                estado = model.Estado,
                rowVersion = model.RowVersion // Envío del sello de concurrencia
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/Equipos/{id}", content);

            if (response.IsSuccessStatusCode) return (true, null, false);

            var errorMsg = await response.Content.ReadAsStringAsync();
            bool esConflicto = response.StatusCode == HttpStatusCode.Conflict;
            return (false, string.IsNullOrWhiteSpace(errorMsg) ? "Error al actualizar el equipo." : errorMsg, esConflicto);
        }

        public async Task<(bool Exito, string? Error)> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Equipos/{id}");
            if (response.IsSuccessStatusCode) return (true, null);

            var errorMsg = await response.Content.ReadAsStringAsync();
            return (false, response.StatusCode == HttpStatusCode.Conflict
                ? "No se puede eliminar el equipo porque tiene registros de préstamos."
                : errorMsg);
        }
    }
}