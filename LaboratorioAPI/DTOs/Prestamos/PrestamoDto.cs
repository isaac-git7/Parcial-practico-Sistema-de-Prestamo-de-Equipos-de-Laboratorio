namespace LaboratorioAPI.DTOs.Prestamos
{
    // Información devuelta al consultar un préstamo
    public class PrestamoDto
    {
        public int Id { get; set; }
        public int EquipoId { get; set; }
        public string EquipoCodigo { get; set; } = string.Empty;
        public string EquipoNombre { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }
}