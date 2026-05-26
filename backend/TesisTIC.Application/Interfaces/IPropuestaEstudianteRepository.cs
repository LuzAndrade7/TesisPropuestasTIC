using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Interfaces
{
    /// <summary>
    /// HU07 T20: Interfaz para repositorio de asignación de estudiantes a propuestas
    /// </summary>
    public interface IPropuestaEstudianteRepository : IRepository<PropuestaEstudiante>
    {
        /// <summary>
        /// Obtiene los estudiantes asignados a una propuesta
        /// </summary>
        Task<List<PropuestaEstudiante>> GetByPropuestaIdAsync(int propuestaId);

        /// <summary>
        /// Obtiene una asignación específica
        /// </summary>
        Task<PropuestaEstudiante?> GetByPropuestaAndEstudianteAsync(int propuestaId, int estudianteId);

        /// <summary>
        /// Cuenta estudiantes activos asignados a una propuesta
        /// </summary>
        Task<int> CountActivosByPropuestaIdAsync(int propuestaId);

        /// <summary>
        /// Elimina todos los estudiantes asignados a una propuesta
        /// </summary>
        Task<int> DeleteByPropuestaIdAsync(int propuestaId);

        /// <summary>
        /// Verifica si una propuesta tiene estudiantes asignados
        /// </summary>
        Task<bool> TieneEstudiantesAsync(int propuestaId);
    }
}
