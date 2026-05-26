using TesisTIC.Application.DTOs;
using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Interfaces;

/// <summary>
/// Interfaz para operaciones específicas del repositorio de Propuestas
/// </summary>
public interface IPropuestaRepository : IRepository<Propuesta>
{
    /// <summary>
    /// Obtiene propuestas con filtros opcionales
    /// </summary>
    Task<IEnumerable<Propuesta>> GetPropuestasAsync(
        string? estado = null,
        int? profesorId = null);

    /// <summary>
    /// Obtiene una propuesta con todas sus relaciones
    /// </summary>
    Task<Propuesta?> GetPropuestaFullAsync(int id);

    /// <summary>
    /// Obtiene propuestas por profesor
    /// </summary>
    Task<IEnumerable<Propuesta>> GetPropuestasPorProfesorAsync(int profesorId);

    /// <summary>
    /// Cambiar estado de una propuesta
    /// </summary>
    Task<Propuesta?> CambiarEstadoAsync(int id, string nuevoEstado);

    /// <summary>
    /// Obtiene propuestas por estado
    /// </summary>
    Task<IEnumerable<Propuesta>> GetPropuestasPorEstadoAsync(string estado);
}

/// <summary>
/// Interfaz para servicio de propuestas
/// </summary>
public interface IPropuestaService
{
    // LECTURA
    Task<IEnumerable<PropuestaDto>> GetAllAsync();
    Task<PropuestaDto?> GetByIdAsync(int id);
    Task<IEnumerable<PropuestaDto>> GetPorEstadoAsync(string estado);
    Task<IEnumerable<PropuestaDto>> GetPorProfesorAsync(int profesorId);

    // ESCRITURA
    Task<PropuestaDto> CreateAsync(CreatePropuestaDto dto);
    Task<PropuestaDto> UpdateAsync(int id, UpdatePropuestaDto dto);
    Task<bool> DeleteAsync(int id);
    Task<PropuestaDto> CambiarEstadoAsync(int id, UpdateEstadoPropuestaDto dto);
    Task<PropuestaDto> EnviarARevisionAsync(int id, EnviarARevisionDto dto); // HU03 T07
    Task<PropuestaDto> ReenviarDespuesDeObservacionesAsync(int id); // HU04 T12 - Limpiar observaciones y cambiar estado
    Task<PropuestaDto> SolicitarNuevaAprobacionAsync(int id, string motivo); // HU07 T23

    // ASIGNATURAS
    Task<bool> AsignarAsignaturaAsync(int propuestaId, int asignaturaId);
    Task<bool> RemoverAsignaturaAsync(int propuestaId, int asignaturaId);
}
