using LaboratorioMVC.Models;

namespace LaboratorioMVC.Services
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaViewModel>> GetAllAsync();
        Task<CategoriaViewModel?> GetByIdAsync(int id);
        Task<(bool Exito, string? Error)> CreateAsync(CategoriaViewModel model);
        Task<(bool Exito, string? Error)> UpdateAsync(int id, CategoriaViewModel model);
        Task<(bool Exito, string? Error)> DeleteAsync(int id);
    }
}