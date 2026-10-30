using LaboratorioMVC.Models;

namespace LaboratorioMVC.Services
{
    public interface IPrestamoService
    {
        Task<IEnumerable<PrestamoListViewModel>> GetAllAsync(bool soloVencidos);
        Task<(bool Exito, string? Error)> CreateAsync(PrestamoCreateViewModel model);
        Task<(bool Exito, string? Error)> DevolverAsync(int id);
        Task<IEnumerable<UsuarioItemViewModel>> GetUsuariosAsync();
    }
}