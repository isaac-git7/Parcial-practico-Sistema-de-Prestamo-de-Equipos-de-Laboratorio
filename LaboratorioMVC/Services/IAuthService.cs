namespace LaboratorioMVC.Services
{
    public record LoginResultado(bool Exito, string? Mensaje, string? Token, string? Rol, string? Username);

    public interface IAuthService
    {
        Task<LoginResultado> LoginAsync(string username, string password);
    }
}