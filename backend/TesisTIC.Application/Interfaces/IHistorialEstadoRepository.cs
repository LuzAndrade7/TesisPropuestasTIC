using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Interfaces
{
    /// <summary>
    /// HU06 T17: Interfaz para repositorio de historial de estados
    /// Define el contrato para acceso a datos del histórico
    /// </summary>
    public interface IHistorialEstadoRepository : IRepository<HistorialEstado>
    {
        Task<List<HistorialEstado>> GetByPropuestaIdAsync(int propuestaId);
        Task<HistorialEstado?> GetUltimoByPropuestaIdAsync(int propuestaId);
        Task<int> CountByPropuestaIdAsync(int propuestaId);
    }
}
