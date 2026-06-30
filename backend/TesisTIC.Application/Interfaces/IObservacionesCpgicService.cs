using TesisTIC.Application.DTOs;

namespace TesisTIC.Application.Interfaces;

/// <summary>
/// HU04 T10: Interfaz para servicio de observaciones CPGIC
/// Maneja registro, listado y limpieza de observaciones sobre propuestas en revisión
/// </summary>
public interface IObservacionesCpgicService
{
    /// <summary>
    /// Registra una nueva observación sobre una propuesta
    /// Cambia el estado de la propuesta a OBSERVADA automáticamente
    /// </summary>
    Task<ObservacionesCpgicDto> CrearObservacionAsync(CreateObservacionesCpgicDto dto);

    /// <summary>
    /// Obtiene todas las observaciones de una propuesta específica
    /// </summary>
    Task<IEnumerable<ObservacionesCpgicDto>> ObtenerObservacionesPorPropuestaAsync(int propuestaId);

    /// <summary>
    /// Obtiene una observación específica por ID
    /// </summary>
    Task<ObservacionesCpgicDto?> ObtenerObservacionAsync(int observacionId);

    /// <summary>
    /// Elimina una observación específica
    /// Si es la última observación, no cambia el estado (CPGIC puede borrar por error)
    /// </summary>
    Task<bool> EliminarObservacionAsync(int observacionId);

    /// <summary>
    /// Limpia TODAS las observaciones de una propuesta
    /// Se usa cuando la propuesta se reenvía después de correcciones
    /// </summary>
    Task<bool> LimpiarObservacionesPorPropuestaAsync(int propuestaId);

    /// <summary>
    /// Verifica si una propuesta tiene observaciones pendientes
    /// </summary>
    Task<bool> TieneObservacionesAsync(int propuestaId);

    /// <summary>
    /// Cuenta las observaciones de una propuesta
    /// </summary>
    Task<int> ContarObservacionesAsync(int propuestaId);
}
