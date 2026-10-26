using System.ComponentModel.DataAnnotations;

namespace LaboratorioAPI.DTOs.Categorias
{
    // Datos que recibimos del formulario cuando se crea una categoría
    public class CategoriaCreateDto
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
        public string? Descripcion { get; set; }
    }
}