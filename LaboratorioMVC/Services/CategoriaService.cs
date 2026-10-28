using System.Net;
using System.Text;
using System.Text.Json;
using LaboratorioMVC.Models;

namespace LaboratorioMVC.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public CategoriaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IEnumerable<CategoriaViewModel>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("api/Categorias");
            if (!response.IsSuccessStatusCode)
            {
                return Enumerable.Empty<CategoriaViewModel>();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<CategoriaViewModel>>(content, _jsonOptions)
                   ?? Enumerable.Empty<CategoriaViewModel>();
        }

        public async Task<CategoriaViewModel?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/Categorias/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CategoriaViewModel>(content, _jsonOptions);
        }

        public async Task<(bool Exito, string? Error)> CreateAsync(CategoriaViewModel model)
        {
            var json = JsonSerializer.Serialize(new { nombre = model.Nombre, descripcion = model.Descripcion });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Categorias", content);
            if (response.IsSuccessStatusCode) return (true, null);

            var errorMsg = await response.Content.ReadAsStringAsync();
            return (false, string.IsNullOrWhiteSpace(errorMsg) ? "Error al registrar la categoría." : errorMsg);
        }

        public async Task<(bool Exito, string? Error)> UpdateAsync(int id, CategoriaViewModel model)
        {
            var json = JsonSerializer.Serialize(new { nombre = model.Nombre, descripcion = model.Descripcion });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/Categorias/{id}", content);
            if (response.IsSuccessStatusCode) return (true, null);

            var errorMsg = await response.Content.ReadAsStringAsync();
            return (false, string.IsNullOrWhiteSpace(errorMsg) ? "Error al actualizar la categoría." : errorMsg);
        }

        public async Task<(bool Exito, string? Error)> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Categorias/{id}");
            if (response.IsSuccessStatusCode) return (true, null);

            var errorMsg = await response.Content.ReadAsStringAsync();
            return (false, response.StatusCode == HttpStatusCode.Conflict
                ? "No se puede eliminar la categoría porque tiene equipos asociados."
                : errorMsg);
        }
    }
}