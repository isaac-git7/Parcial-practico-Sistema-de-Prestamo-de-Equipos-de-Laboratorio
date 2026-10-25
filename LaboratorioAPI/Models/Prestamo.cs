using System.ComponentModel.DataAnnotations;

namespace LaboratorioAPI.Models
{
    // Registro de la asignación temporal de un equipo a un usuario
    public class Prestamo
    {
        public int Id { get; set; }

        // Equipo prestado
        public int EquipoId { get; set; }
        public Equipo? Equipo { get; set; }

        // Usuario solicitante
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public DateTime FechaPrestamo { get; set; } = DateTime.Now;

        // Fecha límite pactada (no puede superar los 7 días calendario)
        public DateTime FechaVencimiento { get; set; }

        // Se completa cuando el usuario entrega el equipo al laboratorio
        public DateTime? FechaDevolucion { get; set; }

        // Estado del préstamo: "Activo", "Devuelto", "Vencido"
        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Activo";

        [StringLength(250)]
        public string? Observaciones { get; set; }
    }
}