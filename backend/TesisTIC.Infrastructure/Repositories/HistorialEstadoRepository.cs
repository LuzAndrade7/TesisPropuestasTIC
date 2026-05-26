using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TesisTIC.Domain.Entities;
using TesisTIC.Application.Interfaces;
using TesisTIC.Infrastructure.Data;

namespace TesisTIC.Infrastructure.Repositories
{
    /// <summary>
    /// HU06 T17: Implementación del repositorio de historial de estados
    /// </summary>
    public class HistorialEstadoRepository : GenericRepository<HistorialEstado>, IHistorialEstadoRepository
    {
        public HistorialEstadoRepository(TesisTicDbContext context) : base(context) { }

        /// <summary>
        /// Obtiene el histórico completo de una propuesta ordenado por fecha
        /// </summary>
        public async Task<List<HistorialEstado>> GetByPropuestaIdAsync(int propuestaId)
        {
            return await _dbSet
                .Where(h => h.PropuestaId == propuestaId)
                .OrderBy(h => h.FechaCambio)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene el último cambio de estado de una propuesta
        /// </summary>
        public async Task<HistorialEstado?> GetUltimoByPropuestaIdAsync(int propuestaId)
        {
            return await _dbSet
                .Where(h => h.PropuestaId == propuestaId)
                .OrderByDescending(h => h.FechaCambio)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Cuenta el total de cambios de estado de una propuesta
        /// </summary>
        public async Task<int> CountByPropuestaIdAsync(int propuestaId)
        {
            return await _dbSet
                .Where(h => h.PropuestaId == propuestaId)
                .CountAsync();
        }
    }
}
