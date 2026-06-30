using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TesisTIC.Domain.Entities;
using TesisTIC.Application.Interfaces;
using TesisTIC.Infrastructure.Data;

namespace TesisTIC.Infrastructure.Repositories
{
    /// <summary>
    /// HU07 T20: Implementación del repositorio de asignación de estudiantes
    /// </summary>
    public class PropuestaEstudianteRepository : GenericRepository<PropuestaEstudiante>, IPropuestaEstudianteRepository
    {
        public PropuestaEstudianteRepository(TesisTicDbContext context) : base(context) { }

        /// <summary>
        /// Obtiene los estudiantes asignados a una propuesta
        /// </summary>
        public async Task<List<PropuestaEstudiante>> GetByPropuestaIdAsync(int propuestaId)
        {
            return await _dbSet
                .Where(pe => pe.PropuestaId == propuestaId && pe.Estado == "ACTIVO")
                .Include(pe => pe.Estudiante)
                .OrderBy(pe => pe.FechaAsignacion)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene una asignación específica
        /// </summary>
        public async Task<PropuestaEstudiante?> GetByPropuestaAndEstudianteAsync(int propuestaId, int estudianteId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(pe => pe.PropuestaId == propuestaId && pe.EstudianteId == estudianteId);
        }

        /// <summary>
        /// Cuenta estudiantes activos asignados a una propuesta
        /// </summary>
        public async Task<int> CountActivosByPropuestaIdAsync(int propuestaId)
        {
            return await _dbSet
                .Where(pe => pe.PropuestaId == propuestaId && pe.Estado == "ACTIVO")
                .CountAsync();
        }

        /// <summary>
        /// Elimina todos los estudiantes asignados a una propuesta
        /// </summary>
        public async Task<int> DeleteByPropuestaIdAsync(int propuestaId)
        {
            var estudiantes = await _dbSet
                .Where(pe => pe.PropuestaId == propuestaId)
                .ToListAsync();

            _dbSet.RemoveRange(estudiantes);
            await _context.SaveChangesAsync();
            return estudiantes.Count;
        }

        /// <summary>
        /// Verifica si una propuesta tiene estudiantes asignados
        /// </summary>
        public async Task<bool> TieneEstudiantesAsync(int propuestaId)
        {
            return await _dbSet
                .AnyAsync(pe => pe.PropuestaId == propuestaId && pe.Estado == "ACTIVO");
        }
    }
}
