using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;
using TesisTIC.Infrastructure.Data;

namespace TesisTIC.Infrastructure.Repositories;

/// <summary>
/// HU04 T10: Repositorio para operaciones con Observaciones CPGIC
/// Extiende el repositorio genérico con consultas personalizadas
/// </summary>
public class ObservacionesCpgicRepository : GenericRepository<ObservacionesCpgic>, IObservacionesCpgicRepository
{
    public ObservacionesCpgicRepository(TesisTicDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtiene todas las observaciones de una propuesta ordenadas por fecha descendente
    /// </summary>
    public async Task<IEnumerable<ObservacionesCpgic>> GetByPropuestaIdAsync(int propuestaId)
    {
        if (propuestaId <= 0)
            throw new ArgumentException("PropuestaId inválido");

        return await _dbSet
            .Where(o => o.PropuestaId == propuestaId)
            .OrderByDescending(o => o.FechaObservacion)
            .ToListAsync();
    }

    /// <summary>
    /// Verifica si una propuesta tiene observaciones
    /// </summary>
    public async Task<bool> TieneObservacionesAsync(int propuestaId)
    {
        if (propuestaId <= 0)
            throw new ArgumentException("PropuestaId inválido");

        return await _dbSet.AnyAsync(o => o.PropuestaId == propuestaId);
    }

    /// <summary>
    /// Cuenta observaciones de una propuesta
    /// </summary>
    public async Task<int> CountByPropuestaIdAsync(int propuestaId)
    {
        if (propuestaId <= 0)
            throw new ArgumentException("PropuestaId inválido");

        return await _dbSet.CountAsync(o => o.PropuestaId == propuestaId);
    }

    /// <summary>
    /// Elimina todas las observaciones de una propuesta
    /// </summary>
    public async Task<int> DeleteByPropuestaIdAsync(int propuestaId)
    {
        if (propuestaId <= 0)
            throw new ArgumentException("PropuestaId inválido");

        var observaciones = await _dbSet
            .Where(o => o.PropuestaId == propuestaId)
            .ToListAsync();

        if (!observaciones.Any())
            return 0;

        _dbSet.RemoveRange(observaciones);
        await _context.SaveChangesAsync();

        return observaciones.Count;
    }
}
