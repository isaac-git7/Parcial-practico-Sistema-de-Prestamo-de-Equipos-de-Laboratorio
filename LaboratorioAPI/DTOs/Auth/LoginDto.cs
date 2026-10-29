using System.ComponentModel.DataAnnotations;

namespace LaboratorioAPI.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El Username es obligatorio")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = string.Empty;
    }
}