using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Interfaces;

/// <summary>
/// HU07 T20: Interfaz para repositorio de estudiantes
/// Métodos para búsqueda, listado y filtrado de estudiantes
/// </summary>
public interface IEstudianteRepository : IRepository<Estudiante>
{
    /// <summary>
    /// Obtiene todos los estudiantes disponibles
    /// </summary>
    Task<List<Estudiante>> GetAllAvailableAsync();

    /// <summary>
    /// Busca estudiantes por nombre o apellido
    /// </summary>
    Task<List<Estudiante>> SearchAsync(string searchTerm);

    /// <summary>
    /// Obtiene estudiantes que NO están asignados a una propuesta específica
    /// </summary>
    Task<List<Estudiante>> GetNoAsignadosAsync(int propuestaId);

    /// <summary>
    /// Obtiene estudiantes asignados a una propuesta
    /// </summary>
    Task<List<Estudiante>> GetAsignadosAsync(int propuestaId);

    /// <summary>
    /// Obtiene un estudiante por correo
    /// </summary>
    Task<Estudiante?> GetByCorreoAsync(string correo);
}
