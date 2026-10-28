using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LaboratorioMVC.Models
{
    public class EquipoListViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Código")]
        public string CodigoInventario { get; set; } = string.Empty;

        [Display(Name = "Equipo")]
        public string Nombre { get; set; } = string.Empty;

        public string? Marca { get; set; }
        public string? Modelo { get; set; }

        [Display(Name = "Categoría")]
        public string CategoriaNombre { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;
    }

    public class EquipoFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código de inventario es obligatorio.")]
        [StringLength(20)]
        [Display(Name = "Código de Inventario")]
        public string CodigoInventario { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
        [StringLength(100)]
        [Display(Name = "Nombre del Equipo")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Marca { get; set; }

        [StringLength(50)]
        public string? Modelo { get; set; }

        [StringLength(50)]
        [Display(Name = "Número de Serie")]
        public string? Serie { get; set; }

        [Required(ErrorMessage = "Debe asignar una categoría.")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un estado.")]
        public string Estado { get; set; } = "Disponible";

        public string RowVersion { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> CategoriasDisponibles { get; set; } = new List<SelectListItem>();
    }

    // Modelo actualizado con soporte para filtros y paginación de 10 elementos
    public class EquipoFiltroViewModel
    {
        public string? Buscar { get; set; }
        public int? CategoriaId { get; set; }
        public string? Estado { get; set; }

        public IEnumerable<EquipoListViewModel> Equipos { get; set; } = new List<EquipoListViewModel>();
        public IEnumerable<SelectListItem> Categorias { get; set; } = new List<SelectListItem>();

        // Propiedades de paginación
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
        public int TotalRegistros { get; set; }
        public int TamanioPagina { get; set; } = 10;

        public bool TienePaginaAnterior => PaginaActual > 1;
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;
    }
}