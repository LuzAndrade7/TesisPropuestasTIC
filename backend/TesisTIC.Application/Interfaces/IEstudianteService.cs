using TesisTIC.Application.DTOs;

namespace TesisTIC.Application.Interfaces;

/// <summary>
/// Interfaz para servicio de Estudiantes
/// </summary>
public interface IEstudianteService
{
    Task<IEnumerable<EstudianteDto>> GetAllAsync();
    Task<EstudianteDto?> GetByIdAsync(int id);
    Task<EstudianteDto> CreateAsync(CreateUpdateEstudianteDto dto);
    Task<EstudianteDto> UpdateAsync(int id, CreateUpdateEstudianteDto dto);
    Task<bool> DeleteAsync(int id);
}
