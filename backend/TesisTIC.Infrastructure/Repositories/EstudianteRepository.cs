using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;
using TesisTIC.Infrastructure.Persistence;

namespace TesisTIC.Infrastructure.Repositories
{
    public class EstudianteRepository : IEstudianteRepository
    {
        private readonly TesisTICDbContext _context;

        public EstudianteRepository(TesisTICDbContext context)
        {
            _context = context;
        }

        public async Task<Estudiante> ObtenerPorIdAsync(int id)
        {
            return await _context.Estudiantes
                .Include(e => e.PropuestasAsignadas)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Estudiante>> ObtenerTodosAsync()
        {
            return await _context.Estudiantes
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ToListAsync();
        }

        public async Task<Estudiante> ObtenerPorMatriculaAsync(string matricula)
        {
            return await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.Matricula == matricula);
        }

        public async Task<Estudiante> CrearAsync(Estudiante estudiante)
        {
            estudiante.FechaCreacion = DateTime.UtcNow;
            var resultado = await _context.Estudiantes.AddAsync(estudiante);
            await GuardarCambiosAsync();
            return resultado.Entity;
        }

        public async Task<int> GuardarCambiosAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
