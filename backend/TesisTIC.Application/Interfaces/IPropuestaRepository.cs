using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Interfaces
{
    public interface IPropuestaRepository
    {
        Task<Propuesta> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Propuesta>> ObtenerTodasAsync();
        Task<IEnumerable<Propuesta>> ObtenerPorDocenteAsync(int docenteId);
        Task<IEnumerable<Propuesta>> ObtenerPorEstadoAsync(int estadoId);
        Task<Propuesta> CrearAsync(Propuesta propuesta);
        Task<Propuesta> ActualizarAsync(Propuesta propuesta);
        Task<bool> EliminarAsync(int id);
        Task<int> GuardarCambiosAsync();
    }
}
