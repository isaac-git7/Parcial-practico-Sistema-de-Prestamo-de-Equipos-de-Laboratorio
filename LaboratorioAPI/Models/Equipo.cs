using System.ComponentModel.DataAnnotations;

namespace LaboratorioAPI.Models
{
    // Representa el bien físico disponible en el laboratorio
    public class Equipo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código de inventario es obligatorio")]
        [StringLength(20, ErrorMessage = "El código no debe superar 20 caracteres")]
        public string CodigoInventario { get; set; } = string.Empty; // Ejemplo: LAP-001, OSC-010

        [Required(ErrorMessage = "El nombre del equipo es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Marca { get; set; }

        [StringLength(50)]
        public string? Modelo { get; set; }

        [StringLength(50)]
        public string? Serie { get; set; }

        // Estado del equipo como texto: "Disponible", "Prestado", "EnMantenimiento", "DeBaja"
        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Disponible";

        // Conexión con la Categoría a la que pertenece
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        // Concurrencia optimista: sello digital que detecta si dos usuarios
        // intentan modificar o prestar el mismo equipo simultáneamente
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        // Historial de préstamos en los que ha estado este equipo
        public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    }
}