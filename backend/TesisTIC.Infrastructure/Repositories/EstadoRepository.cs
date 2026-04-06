using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;
using TesisTIC.Infrastructure.Persistence;

namespace TesisTIC.Infrastructure.Repositories
{
    public class EstadoRepository : IEstadoRepository
    {
        private readonly TesisTICDbContext _context;

        public EstadoRepository(TesisTICDbContext context)
        {
            _context = context;
        }

        public async Task<Estado> ObtenerPorIdAsync(int id)
        {
            return await _context.Estados.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Estado>> ObtenerTodosAsync()
        {
            return await _context.Estados.OrderBy(e => e.Nombre).ToListAsync();
        }

        public async Task<Estado> ObtenerPorNombreAsync(string nombre)
        {
            return await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == nombre);
        }
    }
}
