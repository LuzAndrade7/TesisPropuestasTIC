using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;
using TesisTIC.Infrastructure.Persistence;

namespace TesisTIC.Infrastructure.Repositories
{
    public class LineaInvestigacionRepository : ILineaInvestigacionRepository
    {
        private readonly TesisTICDbContext _context;

        public LineaInvestigacionRepository(TesisTICDbContext context)
        {
            _context = context;
        }

        public async Task<LineaInvestigacion> ObtenerPorIdAsync(int id)
        {
            return await _context.LineasInvestigacion.FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<LineaInvestigacion>> ObtenerTodoAsync()
        {
            return await _context.LineasInvestigacion.OrderBy(l => l.Nombre).ToListAsync();
        }
    }
}
