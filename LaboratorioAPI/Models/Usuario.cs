using System.ComponentModel.DataAnnotations;

namespace LaboratorioAPI.Models
{
    // Usuarios con acceso al sistema (Administradores, Docentes, Estudiantes)
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty; // Carnet o identificador único

        [Required]
        public string PasswordHash { get; set; } = string.Empty; // Contraseña cifrada

        [Required]
        [StringLength(100)]
        public string NombreCompleto { get; set; } = string.Empty;

        // Rol: "Administrador", "Docente", "Estudiante"
        [Required]
        [StringLength(20)]
        public string Rol { get; set; } = "Estudiante";

        // Préstamos que este usuario ha solicitado
        public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    }
}