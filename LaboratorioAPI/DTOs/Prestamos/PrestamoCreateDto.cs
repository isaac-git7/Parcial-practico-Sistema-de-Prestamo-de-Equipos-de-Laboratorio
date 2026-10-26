using System.ComponentModel.DataAnnotations;

namespace LaboratorioAPI.DTOs.Prestamos
{
    // Datos necesarios para solicitar un préstamo
    public class PrestamoCreateDto
    {
        [Required(ErrorMessage = "Debe especificar el equipo a prestar")]
        public int EquipoId { get; set; }

        [Required(ErrorMessage = "Debe indicar el usuario que solicita el equipo")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "Debe ingresar la fecha límite de devolución")]
        public DateTime FechaVencimiento { get; set; }

        [StringLength(250, ErrorMessage = "Las observaciones no deben superar 250 caracteres")]
        public string? Observaciones { get; set; }
    }
}