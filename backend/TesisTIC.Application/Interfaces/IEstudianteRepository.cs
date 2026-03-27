using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Interfaces
{
    public interface IEstudianteRepository
    {
        Task<Estudiante> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Estudiante>> ObtenerTodosAsync();
        Task<Estudiante> ObtenerPorMatriculaAsync(string matricula);
        Task<Estudiante> CrearAsync(Estudiante estudiante);
        Task<int> GuardarCambiosAsync();
    }
}
