using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;
using TesisTIC.Infrastructure.Data;

namespace TesisTIC.Infrastructure.Repositories;

/// <summary>
/// Repositorio específico para operaciones con Asignaturas
/// </summary>
public class AsignaturaRepository : GenericRepository<Asignatura>, IAsignaturaRepository
{
    public AsignaturaRepository(TesisTicDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtiene una asignatura por su código único
    /// </summary>
    public async Task<Asignatura?> GetByCodigoAsync(string codigo)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Codigo == codigo);
    }
}
