using TesisTIC.Application.DTOs;
using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Interfaces;

/// <summary>
/// Interfaz para repositorio de Asignaturas
/// </summary>
public interface IAsignaturaRepository : IRepository<Asignatura>
{
    /// <summary>
    /// Obtiene una asignatura por código
    /// </summary>
    Task<Asignatura?> GetByCodigoAsync(string codigo);
}

/// <summary>
/// Interfaz para servicio de Asignaturas
/// </summary>
public interface IAsignaturaService
{
    Task<IEnumerable<AsignaturaDto>> GetAllAsync();
    Task<AsignaturaDto?> GetByIdAsync(int id);
    Task<AsignaturaDto> CreateAsync(CreateUpdateAsignaturaDto dto);
    Task<AsignaturaDto> UpdateAsync(int id, CreateUpdateAsignaturaDto dto);
    Task<bool> DeleteAsync(int id);
}
