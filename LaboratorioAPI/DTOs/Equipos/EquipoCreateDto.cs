using System.ComponentModel.DataAnnotations;

namespace LaboratorioAPI.DTOs.Equipos
{
    // Datos requeridos para registrar un nuevo equipo físico
    public class EquipoCreateDto
    {
        [Required(ErrorMessage = "El código de inventario es obligatorio")]
        [StringLength(20, ErrorMessage = "El código no debe superar 20 caracteres")]
        public string CodigoInventario { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Marca { get; set; }

        [StringLength(50)]
        public string? Modelo { get; set; }

        [StringLength(50)]
        public string? Serie { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría")]
        public int CategoriaId { get; set; }

        // Estado inicial sugerido (por defecto Disponible)
        public string Estado { get; set; } = "Disponible";
    }
}