using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;
using TesisTIC.Infrastructure.Data;

namespace TesisTIC.Infrastructure.Repositories;

/// <summary>
/// Repositorio específico para operaciones con Docentes
/// </summary>
public class DocenteRepository : GenericRepository<Docente>, IDocenteRepository
{
    public DocenteRepository(TesisTicDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtiene un docente por su correo
    /// </summary>
    public async Task<Docente?> GetByCorreoAsync(string correo)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Correo == correo);
    }

    /// <summary>
    /// Obtiene todos los docentes que tienen propuestas asignadas
    /// </summary>
    public async Task<IEnumerable<Docente>> GetConPropuestasAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Include(d => d.Propuestas)
            .Where(d => d.Propuestas.Any())
            .ToListAsync();
    }
}
