using LaboratorioAPI.Models;

namespace LaboratorioAPI.Services
{
    public interface ITokenService
    {
        (string Token, DateTime Expira) GenerarToken(Usuario usuario);
    }
}