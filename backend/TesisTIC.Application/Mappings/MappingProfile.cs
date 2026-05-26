using AutoMapper;
using TesisTIC.Application.DTOs;
using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Mappings;

/// <summary>
/// Perfil AutoMapper para mapeos de Propuestas
/// </summary>
public class PropuestaMappingProfile : Profile
{
    public PropuestaMappingProfile()
    {
        // Propuesta -> PropuestaDto
        CreateMap<Propuesta, PropuestaDto>()
            .ForMember(dest => dest.Profesor,
                opt => opt.MapFrom(src => src.Profesor))
            .ForMember(dest => dest.Asignaturas,
                opt => opt.MapFrom(src => src.PropuestasAsignaturas
                    .Select(pa => pa.Asignatura)
                    .ToList()));

        // T04: Propuesta -> PropuestaResumenDto (para tablero HU02)
        CreateMap<Propuesta, PropuestaResumenDto>()
            .ForMember(dest => dest.Profesor,
                opt => opt.MapFrom(src => src.Profesor != null
                    ? new DocenteResumenDto
                    {
                        Id = src.Profesor.Id,
                        NombreCompleto = $"{src.Profesor.Nombres} {src.Profesor.Apellidos}".Trim(),
                        Correo = src.Profesor.Correo
                    }
                    : null))
            .ForMember(dest => dest.Asignaturas,
                opt => opt.MapFrom(src => src.PropuestasAsignaturas
                    .Select(pa => pa.Asignatura)
                    .ToList()));

        // CreatePropuestaDto -> Propuesta
        CreateMap<CreatePropuestaDto, Propuesta>()
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(_ => "BORRADOR"))
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.FechaActualizacion, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // UpdatePropuestaDto -> Propuesta
        CreateMap<UpdatePropuestaDto, Propuesta>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}

/// <summary>
/// Perfil AutoMapper para mapeos de Docentes
/// </summary>
public class DocenteMappingProfile : Profile
{
    public DocenteMappingProfile()
    {
        CreateMap<Docente, DocenteDto>();
        CreateMap<CreateUpdateDocenteDto, Docente>();
    }
}

/// <summary>
/// Perfil AutoMapper para mapeos de Asignaturas
/// </summary>
public class AsignaturaMappingProfile : Profile
{
    public AsignaturaMappingProfile()
    {
        CreateMap<Asignatura, AsignaturaDto>();
        CreateMap<CreateUpdateAsignaturaDto, Asignatura>();
    }
}

/// <summary>
/// Perfil AutoMapper para mapeos de Estudiantes
/// </summary>
public class EstudianteMappingProfile : Profile
{
    public EstudianteMappingProfile()
    {
        CreateMap<Estudiante, EstudianteDto>();
        CreateMap<CreateUpdateEstudianteDto, Estudiante>();
    }
}

/// <summary>
/// Perfil AutoMapper para mapeos de Componentes
/// </summary>
public class ComponenteMappingProfile : Profile
{
    public ComponenteMappingProfile()
    {
        CreateMap<Componente, ComponenteDto>()
            .ForMember(dest => dest.Estudiante,
                opt => opt.MapFrom(src => src.Estudiante))
            .ForMember(dest => dest.Actividades,
                opt => opt.MapFrom(src => src.Actividades.ToList()))
            .ForMember(dest => dest.ProductosEsperados,
                opt => opt.MapFrom(src => src.ProductosEsperados.ToList()));

        CreateMap<CreateComponenteDto, Componente>();
        CreateMap<UpdateComponenteDto, Componente>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}

/// <summary>
/// Perfil AutoMapper para mapeos de Actividades
/// </summary>
public class ActividadMappingProfile : Profile
{
    public ActividadMappingProfile()
    {
        CreateMap<Actividad, ActividadDto>();
        CreateMap<CreateActividadDto, Actividad>();
        CreateMap<UpdateActividadDto, Actividad>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}

/// <summary>
/// Perfil AutoMapper para mapeos de Productos Esperados
/// </summary>
public class ProductoEsperadoMappingProfile : Profile
{
    public ProductoEsperadoMappingProfile()
    {
        CreateMap<ProductoEsperado, ProductoEsperadoDto>();
        CreateMap<CreateProductoEsperadoDto, ProductoEsperado>();
        CreateMap<UpdateProductoEsperadoDto, ProductoEsperado>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}

/// <summary>
/// Perfil AutoMapper para mapeos de Observaciones CPGIC
/// </summary>
public class ObservacionesCpgicMappingProfile : Profile
{
    public ObservacionesCpgicMappingProfile()
    {
        CreateMap<ObservacionesCpgic, ObservacionesCpgicDto>();
        CreateMap<CreateObservacionesCpgicDto, ObservacionesCpgic>();
    }
}

/// <summary>
/// Perfil AutoMapper para mapeos de Aprobaciones CPGIC
/// </summary>
public class AprobacionesCpgicMappingProfile : Profile
{
    public AprobacionesCpgicMappingProfile()
    {
        CreateMap<AprobacionesCpgic, AprobacionesCpgicDto>();
        CreateMap<CreateAprobacionesCpgicDto, AprobacionesCpgic>();
        CreateMap<UpdateAprobacionesCpgicDto, AprobacionesCpgic>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
