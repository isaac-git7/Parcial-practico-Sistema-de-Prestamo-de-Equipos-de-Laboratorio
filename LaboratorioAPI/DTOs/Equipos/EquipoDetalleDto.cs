namespace LaboratorioAPI.DTOs.Equipos
{
    // Información detallada para la pantalla de "Ver Detalles"
    public class EquipoDetalleDto
    {
        public int Id { get; set; }
        public string CodigoInventario { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Serie { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;

        // El sello de versión en formato de texto Base64 para transportarlo en JSON
        public string RowVersion { get; set; } = string.Empty;
    }
}
