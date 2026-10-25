using System.ComponentModel.DataAnnotations;

namespace LaboratorioAPI.Models
{
    // Clasificación de los equipos del laboratorio (Laptops, Proyectores, etc.)
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres")]
        public string? Descripcion { get; set; }

        // Relación: Una categoría agrupa a muchos equipos
        public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
    }
}