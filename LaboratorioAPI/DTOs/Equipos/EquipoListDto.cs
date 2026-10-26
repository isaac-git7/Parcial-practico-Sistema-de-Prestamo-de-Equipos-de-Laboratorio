namespace LaboratorioAPI.DTOs.Equipos
{
    // Información resumida que viaja a la tabla principal
    public class EquipoListDto
    {
        public int Id { get; set; }
        public string CodigoInventario { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string Estado { get; set; } = string.Empty;

        // En lugar de enviar toda la entidad Categoria, mandamos solo su nombre
        public string CategoriaNombre { get; set; } = string.Empty;
    }
}