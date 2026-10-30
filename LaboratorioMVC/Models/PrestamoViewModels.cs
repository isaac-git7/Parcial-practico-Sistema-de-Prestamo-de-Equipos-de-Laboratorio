using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LaboratorioMVC.Models
{
    public class PrestamoListViewModel
    {
        public int Id { get; set; }
        public string EquipoCodigo { get; set; } = string.Empty;
        public string EquipoNombre { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }

        public bool EstaVencido => FechaDevolucion == null && FechaVencimiento.Date < DateTime.Today;
    }

    public class PrestamoCreateViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un equipo.")]
        [Display(Name = "Equipo")]
        public int EquipoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un usuario.")]
        [Display(Name = "Usuario solicitante")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "Debe indicar la fecha de vencimiento.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de vencimiento")]
        public DateTime FechaVencimiento { get; set; }

        [StringLength(250, ErrorMessage = "Las observaciones no deben superar 250 caracteres.")]
        public string? Observaciones { get; set; }

        public IEnumerable<SelectListItem> EquiposDisponibles { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Usuarios { get; set; } = new List<SelectListItem>();
    }

    public class UsuarioItemViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
    }
}