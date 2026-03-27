using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Interfaces
{
    public interface IAsignaturaRepository
    {
        Task<Asignatura> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Asignatura>> ObtenerTodosAsync();
    }
}
