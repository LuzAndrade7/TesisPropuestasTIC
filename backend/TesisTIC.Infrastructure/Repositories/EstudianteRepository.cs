using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;
using TesisTIC.Infrastructure.Data;
using System.Linq;

namespace TesisTIC.Infrastructure.Repositories;

/// <summary>
/// Repositorio específico para operaciones con Estudiantes
/// </summary>
public class EstudianteRepository : GenericRepository<Estudiante>, IEstudianteRepository
{
    public EstudianteRepository(TesisTicDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtiene un estudiante por su correo
    /// </summary>
    public async Task<Estudiante?> GetByCorreoAsync(string correo)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Correo == correo);
    }

    /// <summary>
    /// Obtiene todos los estudiantes disponibles
    /// </summary>
    public async Task<List<Estudiante>> GetAllAvailableAsync()
    {
        return await _dbSet
            .OrderBy(e => e.Apellidos)
            .ThenBy(e => e.Nombres)
            .ToListAsync();
    }

    /// <summary>
    /// Busca estudiantes por nombre o apellido
    /// </summary>
    public async Task<List<Estudiante>> SearchAsync(string searchTerm)
    {
        var lowerSearch = searchTerm.ToLower();
        return await _dbSet
            .Where(e => e.Nombres.ToLower().Contains(lowerSearch) ||
                       e.Apellidos.ToLower().Contains(lowerSearch) ||
                       (e.Correo != null && e.Correo.ToLower().Contains(lowerSearch)))
            .OrderBy(e => e.Apellidos)
            .ThenBy(e => e.Nombres)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene estudiantes que NO están asignados a una propuesta específica
    /// </summary>
    public async Task<List<Estudiante>> GetNoAsignadosAsync(int propuestaId)
    {
        return await _dbSet
            .Where(e => !e.PropuestaEstudiantes.Any(pe => pe.PropuestaId == propuestaId && pe.Estado == "ACTIVO"))
            .OrderBy(e => e.Apellidos)
            .ThenBy(e => e.Nombres)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene estudiantes asignados a una propuesta
    /// </summary>
    public async Task<List<Estudiante>> GetAsignadosAsync(int propuestaId)
    {
        return await _dbSet
            .Where(e => e.PropuestaEstudiantes.Any(pe => pe.PropuestaId == propuestaId && pe.Estado == "ACTIVO"))
            .OrderBy(e => e.Apellidos)
            .ThenBy(e => e.Nombres)
            .ToListAsync();
    }
}
