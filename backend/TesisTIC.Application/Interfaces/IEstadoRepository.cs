using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Interfaces
{
    public interface IEstadoRepository
    {
        Task<Estado> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Estado>> ObtenerTodosAsync();
        Task<Estado> ObtenerPorNombreAsync(string nombre);
    }
}
