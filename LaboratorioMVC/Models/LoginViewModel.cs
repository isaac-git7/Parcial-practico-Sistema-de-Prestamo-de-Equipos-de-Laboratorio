using System.ComponentModel.DataAnnotations;

namespace LaboratorioMVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El Username es obligatorio")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;
    }
}