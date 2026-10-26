using AutoMapper;
using LaboratorioAPI.DTOs.Categorias;
using LaboratorioAPI.DTOs.Equipos;
using LaboratorioAPI.Models;
using LaboratorioAPI.DTOs.Prestamos;

namespace LaboratorioAPI.Mappings
{
    // Aquí definimos las reglas de conversión automática entre las clases de la base de datos y los DTOs
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // === Mapeos de Categoría ===
            CreateMap<Categoria, CategoriaDto>();
            CreateMap<CategoriaCreateDto, Categoria>();

            // === Mapeos de Equipo ===
            // Convierte la entidad a la lista de la grilla, sacando el nombre de la categoría relacionada
            CreateMap<Equipo, EquipoListDto>()
                .ForMember(dest => dest.CategoriaNombre, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nombre : string.Empty));

            // Convierte la entidad al detalle y transforma el arreglo de bytes del RowVersion en texto Base64
            CreateMap<Equipo, EquipoDetalleDto>()
                .ForMember(dest => dest.CategoriaNombre, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nombre : string.Empty))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => Convert.ToBase64String(src.RowVersion)));

            // Convierte los datos que vienen del formulario a la entidad de la base de datos
            CreateMap<EquipoCreateDto, Equipo>();

            // Convierte el DTO de actualización a la entidad, transformando el texto Base64 de vuelta a arreglo de bytes
            CreateMap<EquipoUpdateDto, Equipo>()
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => Convert.FromBase64String(src.RowVersion)));

            CreateMap<Prestamo, PrestamoDto>()
    .ForMember(dest => dest.EquipoCodigo, opt => opt.MapFrom(src => src.Equipo != null ? src.Equipo.CodigoInventario : string.Empty))
    .ForMember(dest => dest.EquipoNombre, opt => opt.MapFrom(src => src.Equipo != null ? src.Equipo.Nombre : string.Empty))
    .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.NombreCompleto : string.Empty));

            CreateMap<PrestamoCreateDto, Prestamo>();
        }
    }
}