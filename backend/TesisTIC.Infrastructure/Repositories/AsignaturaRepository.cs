using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;
using TesisTIC.Infrastructure.Persistence;

namespace TesisTIC.Infrastructure.Repositories
{
    public class AsignaturaRepository : IAsignaturaRepository
    {
        private readonly TesisTICDbContext _context;

        public AsignaturaRepository(TesisTICDbContext context)
        {
            _context = context;
        }

        public async Task<Asignatura> ObtenerPorIdAsync(int id)
        {
            return await _context.Asignaturas.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Asignatura>> ObtenerTodosAsync()
        {
            return await _context.Asignaturas.OrderBy(a => a.Nombre).ToListAsync();
        }
    }
}
