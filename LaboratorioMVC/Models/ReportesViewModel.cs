namespace LaboratorioMVC.Models
{
    // Métricas numéricas del panel superior
    public class ResumenDashboardViewModel
    {
        public int TotalEquipos { get; set; }
        public int EquiposDisponibles { get; set; }
        public int EquiposPrestados { get; set; }
        public int EquiposEnMantenimiento { get; set; }
        public int TotalPrestamosActivos { get; set; }
        public int PrestamosVencidos { get; set; }
    }

    // Fila para la tabla de mora
    public class PrestamoVencidoViewModel
    {
        public int PrestamoId { get; set; }
        public string CodigoInventario { get; set; } = string.Empty;
        public string NombreEquipo { get; set; } = string.Empty;
        public string CarnetOUsuario { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int DiasRetraso { get; set; }
    }

    // Fila para el Top 5
    public class EquipoTopPrestamosViewModel
    {
        public int EquipoId { get; set; }
        public string CodigoInventario { get; set; } = string.Empty;
        public string NombreEquipo { get; set; } = string.Empty;
        public string CategoriaNombre { get; set; } = string.Empty;
        public int TotalPrestamos { get; set; }
    }

    // Modelo contenedor para la vista Index de Reportes
    public class TableroReportesViewModel
    {
        public ResumenDashboardViewModel Resumen { get; set; } = new();
        public IEnumerable<PrestamoVencidoViewModel> Vencidos { get; set; } = new List<PrestamoVencidoViewModel>();
        public IEnumerable<EquipoTopPrestamosViewModel> TopEquipos { get; set; } = new List<EquipoTopPrestamosViewModel>();
    }

    // Modelo para la vista Historial por Usuario (Reporte 4)
    public class HistorialUsuarioViewModel
    {
        public string? TerminoBusqueda { get; set; }
        public bool RealizoBusqueda { get; set; } = false;
        public string? MensajeError { get; set; }

        public int UsuarioId { get; set; }
        public string Carnet { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public int TotalSolicitados { get; set; }
        public int TotalActivos { get; set; }
        public int TotalDevueltos { get; set; }

        public List<PrestamoHistorialItemViewModel> Prestamos { get; set; } = new();
    }

    public class PrestamoHistorialItemViewModel
    {
        public int PrestamoId { get; set; }
        public string CodigoInventario { get; set; } = string.Empty;
        public string NombreEquipo { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }
}