using System.Net;
using System.Text.Json;
using LaboratorioMVC.Models;

namespace LaboratorioMVC.Services
{
    public class ReporteService : IReporteService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ReporteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<TableroReportesViewModel> ObtenerTableroCompletoAsync()
        {
            var tablero = new TableroReportesViewModel();

            // 1. Métricas KPI
            var resDashboard = await _httpClient.GetAsync("api/Reportes/dashboard");
            if (resDashboard.IsSuccessStatusCode)
            {
                var content = await resDashboard.Content.ReadAsStringAsync();
                tablero.Resumen = JsonSerializer.Deserialize<ResumenDashboardViewModel>(content, _jsonOptions) ?? new();
            }

            // 2. Préstamos en mora
            var resVencidos = await _httpClient.GetAsync("api/Reportes/vencidos");
            if (resVencidos.IsSuccessStatusCode)
            {
                var content = await resVencidos.Content.ReadAsStringAsync();
                tablero.Vencidos = JsonSerializer.Deserialize<IEnumerable<PrestamoVencidoViewModel>>(content, _jsonOptions)
                                   ?? Enumerable.Empty<PrestamoVencidoViewModel>();
            }

            // 3. Top 5 de equipos
            var resTop = await _httpClient.GetAsync("api/Reportes/mas-prestados");
            if (resTop.IsSuccessStatusCode)
            {
                var content = await resTop.Content.ReadAsStringAsync();
                tablero.TopEquipos = JsonSerializer.Deserialize<IEnumerable<EquipoTopPrestamosViewModel>>(content, _jsonOptions)
                                     ?? Enumerable.Empty<EquipoTopPrestamosViewModel>();
            }

            return tablero;
        }

        public async Task<HistorialUsuarioViewModel> ObtenerHistorialPorUsuarioAsync(string termino)
        {
            var resultado = new HistorialUsuarioViewModel
            {
                TerminoBusqueda = termino,
                RealizoBusqueda = true
            };

            if (string.IsNullOrWhiteSpace(termino))
            {
                resultado.MensajeError = "Por favor ingrese un carnet o nombre para realizar la búsqueda.";
                return resultado;
            }

            var url = $"api/Reportes/historial-usuario?termino={Uri.EscapeDataString(termino)}";
            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                resultado.MensajeError = $"No se encontró ningún usuario con el carnet o nombre '{termino}'.";
                return resultado;
            }

            if (!response.IsSuccessStatusCode)
            {
                resultado.MensajeError = "Ocurrió un error al consultar el historial del usuario.";
                return resultado;
            }

            var content = await response.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<HistorialUsuarioViewModel>(content, _jsonOptions);

            if (dto != null)
            {
                dto.TerminoBusqueda = termino;
                dto.RealizoBusqueda = true;
                return dto;
            }

            resultado.MensajeError = "Error al procesar la información del usuario.";
            return resultado;
        }
    }
}