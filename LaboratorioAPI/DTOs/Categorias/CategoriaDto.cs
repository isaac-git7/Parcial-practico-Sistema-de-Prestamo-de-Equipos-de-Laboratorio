namespace LaboratorioAPI.DTOs.Categorias
{
    // Datos que enviamos al frontend cuando consulta categorías
    public class CategoriaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}