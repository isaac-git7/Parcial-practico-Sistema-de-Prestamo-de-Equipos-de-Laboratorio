using System.ComponentModel.DataAnnotations;

namespace LaboratorioAPI.DTOs.Equipos
{
    // Datos que enviamos cuando editamos un equipo existente
    public class EquipoUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Marca { get; set; }

        [StringLength(50)]
        public string? Modelo { get; set; }

        [StringLength(50)]
        public string? Serie { get; set; }

        [Required(ErrorMessage = "Debe pertenecer a una categoría")]
        public int CategoriaId { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Disponible";

        // Sello digital: si no coincide con el de la base de datos,
        // significa que alguien más actualizó el registro antes
        [Required(ErrorMessage = "El RowVersion es obligatorio para verificar cambios")]
        public string RowVersion { get; set; } = string.Empty;
    }
}