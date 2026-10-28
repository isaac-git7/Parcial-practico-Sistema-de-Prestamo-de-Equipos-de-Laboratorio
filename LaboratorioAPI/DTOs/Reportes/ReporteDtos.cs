namespace LaboratorioAPI.DTOs.Reportes
{
    // Métricas generales para las tarjetas del Dashboard
    public class ResumenDashboardDto
    {
        public int TotalEquipos { get; set; }
        public int EquiposDisponibles { get; set; }
        public int EquiposPrestados { get; set; }
        public int EquiposEnMantenimiento { get; set; }
        public int TotalPrestamosActivos { get; set; }
        public int PrestamosVencidos { get; set; }
    }

    // Listado de préstamos que superaron su fecha de vencimiento sin devolver
    public class PrestamoVencidoDto
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

    // Top 5 de equipos con mayor demanda
    public class EquipoTopPrestamosDto
    {
        public int EquipoId { get; set; }
        public string CodigoInventario { get; set; } = string.Empty;
        public string NombreEquipo { get; set; } = string.Empty;
        public string CategoriaNombre { get; set; } = string.Empty;
        public int TotalPrestamos { get; set; }
    }

    // Reporte 4: Ficha del usuario con resumen numérico
    public class HistorialUsuarioDto
    {
        public int UsuarioId { get; set; }
        public string Carnet { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public int TotalSolicitados { get; set; }
        public int TotalActivos { get; set; }
        public int TotalDevueltos { get; set; }

        public List<PrestamoHistorialItemDto> Prestamos { get; set; } = new();
    }

    // Reporte 4: Cada fila de préstamo asociado al usuario
    public class PrestamoHistorialItemDto
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