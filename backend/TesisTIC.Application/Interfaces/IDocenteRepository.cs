using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Interfaces
{
    public interface IDocenteRepository
    {
        Task<Docente> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Docente>> ObtenerTodosAsync();
        Task<Docente> ObtenerPorCorreoAsync(string correo);
        Task<Docente> CrearAsync(Docente docente);
        Task<int> GuardarCambiosAsync();
    }
}
