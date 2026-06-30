using TesisTIC.Application.DTOs;

namespace TesisTIC.Application.Interfaces;

/// <summary>
/// HU07 T20: Interfaz para servicio de asignación de estudiantes a propuestas
/// </summary>
public interface IPropuestaEstudianteService
{
    // Lectura
    Task<List<PropuestaEstudianteDto>> ObtenerEstudiantesPorPropuestaAsync(int propuestaId);
    Task<List<EstudianteDto>> ObtenerTodosEstudiantesAsync();
    Task<List<EstudianteDto>> BuscarEstudiantesAsync(string searchTerm);
    Task<List<EstudianteDto>> ObtenerEstudiantesDisponiblesAsync(int propuestaId);

    // Escritura
    Task<List<PropuestaEstudianteDto>> AsignarEstudiantesAsync(int propuestaId, List<int> estudianteIds, string? motivo = null, string? realizadoPor = null);
    Task<List<PropuestaEstudianteDto>> ActualizarEstudiantesAsync(int propuestaId, List<int> nuevosEstudianteIds, string motivo, string? realizadoPor = null);
    Task<int> ObtenerConteoEstudiantesAsync(int propuestaId);
    Task<bool> TieneEstudiantesAsync(int propuestaId);
}
