using System.ComponentModel.DataAnnotations;

namespace LaboratorioAPI.DTOs.Auth
{
    public class RegisterDto
    {
        [Required, StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required, MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Admin|Docente|Estudiante)$", ErrorMessage = "El rol debe ser Admin, Docente o Estudiante")]
        public string Rol { get; set; } = "Estudiante";
    }
}