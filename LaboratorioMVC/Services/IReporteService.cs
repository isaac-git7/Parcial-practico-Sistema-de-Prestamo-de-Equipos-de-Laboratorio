using LaboratorioMVC.Models;

namespace LaboratorioMVC.Services
{
    public interface IReporteService
    {
        Task<TableroReportesViewModel> ObtenerTableroCompletoAsync();
        Task<HistorialUsuarioViewModel> ObtenerHistorialPorUsuarioAsync(string termino);
    }
}