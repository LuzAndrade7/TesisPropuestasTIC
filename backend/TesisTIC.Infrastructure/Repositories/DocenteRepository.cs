using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;

namespace TesisTIC.Infrastructure.Repositories
{
    public class DocenteRepository : IDocenteRepository
    {
        private readonly TesisTICDbContext _context;

        public DocenteRepository(TesisTICDbContext context)
        {
            _context = context;
        }

        public async Task<Docente> ObtenerPorIdAsync(int id)
        {
            return await _context.Docentes.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Docente>> ObtenerTodosAsync()
        {
            return await _context.Docentes
                .Where(d => d.Activo)
                .OrderBy(d => d.Apellido)
                .ThenBy(d => d.Nombre)
                .ToListAsync();
        }

        public async Task<Docente> ObtenerPorCorreoAsync(string correo)
        {
            return await _context.Docentes.FirstOrDefaultAsync(d => d.CorreoInstitucional == correo);
        }

        public async Task<Docente> CrearAsync(Docente docente)
        {
            docente.FechaCreacion = DateTime.UtcNow;
            docente.Activo = true;
            var resultado = await _context.Docentes.AddAsync(docente);
            await GuardarCambiosAsync();
            return resultado.Entity;
        }

        public async Task<int> GuardarCambiosAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
