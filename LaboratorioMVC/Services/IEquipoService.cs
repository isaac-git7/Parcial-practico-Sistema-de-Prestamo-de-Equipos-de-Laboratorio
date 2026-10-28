using LaboratorioMVC.Models;

namespace LaboratorioMVC.Services
{
    public interface IEquipoService
    {
        Task<IEnumerable<EquipoListViewModel>> GetAllAsync(string? buscar, int? categoriaId, string? estado);
        Task<EquipoFormViewModel?> GetByIdAsync(int id);
        Task<(bool Exito, string? Error, bool EsConflicto)> CreateAsync(EquipoFormViewModel model);
        Task<(bool Exito, string? Error, bool EsConflicto)> UpdateAsync(int id, EquipoFormViewModel model);
        Task<(bool Exito, string? Error)> DeleteAsync(int id);
    }
}