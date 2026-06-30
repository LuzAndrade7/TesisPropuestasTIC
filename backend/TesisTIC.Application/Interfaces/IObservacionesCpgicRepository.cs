using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Interfaces;

/// <summary>
/// HU04 T10: Interfaz del repositorio para operaciones de observaciones CPGIC
/// </summary>
public interface IObservacionesCpgicRepository : IRepository<ObservacionesCpgic>
{
    /// <summary>
    /// Obtener todas las observaciones de una propuesta
    /// </summary>
    Task<IEnumerable<ObservacionesCpgic>> GetByPropuestaIdAsync(int propuestaId);

    /// <summary>
    /// Verificar si propuesta tiene observaciones
    /// </summary>
    Task<bool> TieneObservacionesAsync(int propuestaId);

    /// <summary>
    /// Contar observaciones por propuesta
    /// </summary>
    Task<int> CountByPropuestaIdAsync(int propuestaId);

    /// <summary>
    /// Eliminar todas las observaciones de una propuesta
    /// </summary>
    Task<int> DeleteByPropuestaIdAsync(int propuestaId);
}
