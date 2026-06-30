using TesisTIC.Application.DTOs;
using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Interfaces;

/// <summary>
/// Interfaz para repositorio de Docentes
/// </summary>
public interface IDocenteRepository : IRepository<Docente>
{
    /// <summary>
    /// Obtiene un docente por correo
    /// </summary>
    Task<Docente?> GetByCorreoAsync(string correo);

    /// <summary>
    /// Obtiene docentes con propuestas
    /// </summary>
    Task<IEnumerable<Docente>> GetConPropuestasAsync();
}

/// <summary>
/// Interfaz para servicio de Docentes
/// </summary>
public interface IDocenteService
{
    Task<IEnumerable<DocenteDto>> GetAllAsync();
    Task<DocenteDto?> GetByIdAsync(int id);
    Task<DocenteDto> CreateAsync(CreateUpdateDocenteDto dto);
    Task<DocenteDto> UpdateAsync(int id, CreateUpdateDocenteDto dto);
    Task<bool> DeleteAsync(int id);
}
