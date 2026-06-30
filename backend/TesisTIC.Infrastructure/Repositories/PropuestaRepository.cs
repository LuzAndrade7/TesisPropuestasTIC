using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.Interfaces;
using TesisTIC.Application.Services;
using TesisTIC.Domain.Entities;
using TesisTIC.Infrastructure.Data;

namespace TesisTIC.Infrastructure.Repositories
{
    public class PropuestaRepository : IPropuestaRepository
    {
        private readonly TesisTicDbContext _context;

        public PropuestaRepository(TesisTicDbContext context)
        {
            _context = context;
        }

        // IReadRepository<Propuesta> methods
        public async Task<IEnumerable<Propuesta>> GetAllAsync()
        {
            return await _context.Propuestas
                .Include(p => p.Profesor)
                .Include(p => p.PropuestasAsignaturas)
                .ThenInclude(pa => pa.Asignatura)
                .Include(p => p.Observaciones)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
        }

        public async Task<Propuesta?> GetByIdAsync(int id)
        {
            return await _context.Propuestas
                .Include(p => p.Profesor)
                .Include(p => p.PropuestasAsignaturas)
                .ThenInclude(pa => pa.Asignatura)
                .Include(p => p.Observaciones)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Propuestas.CountAsync();
        }

        // IWriteRepository<Propuesta> methods
        public async Task<Propuesta> CreateAsync(Propuesta entity)
        {
            entity.FechaCreacion = FechaEcuador.Ahora();
            var resultado = await _context.Propuestas.AddAsync(entity);
            await SaveChangesAsync();
            return resultado.Entity;
        }

        public async Task<Propuesta> UpdateAsync(Propuesta entity)
        {
            entity.FechaActualizacion = FechaEcuador.Ahora();
            _context.Propuestas.Update(entity);
            await SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var propuesta = await _context.Propuestas.FindAsync(id);
            if (propuesta == null)
                return false;

            _context.Propuestas.Remove(propuesta);
            await SaveChangesAsync();
            return true;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // IPropuestaRepository specific methods
        public async Task<IEnumerable<Propuesta>> GetPropuestasAsync(string? estado = null, int? profesorId = null)
        {
            var query = _context.Propuestas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(p => p.Estado == estado);

            if (profesorId.HasValue)
                query = query.Where(p => p.ProfesorId == profesorId);

            return await query
                .Include(p => p.Profesor)
                .Include(p => p.PropuestasAsignaturas)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
        }

        public async Task<Propuesta?> GetPropuestaFullAsync(int id)
        {
            return await _context.Propuestas
                .Include(p => p.Profesor)
                .Include(p => p.PropuestasAsignaturas)
                .ThenInclude(pa => pa.Asignatura)
                .Include(p => p.Observaciones)
                .Include(p => p.AprobacionCpgic)
                .Include(p => p.Componentes)
                .ThenInclude(c => c.Estudiante)
                .Include(p => p.Componentes)
                .ThenInclude(c => c.Actividades)
                .Include(p => p.Componentes)
                .ThenInclude(c => c.ProductosEsperados)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Propuesta>> GetPropuestasPorProfesorAsync(int profesorId)
        {
            return await _context.Propuestas
                .Where(p => p.ProfesorId == profesorId)
                .Include(p => p.Profesor)
                .Include(p => p.PropuestasAsignaturas)
                .ThenInclude(pa => pa.Asignatura)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
        }

        public async Task<Propuesta?> CambiarEstadoAsync(int id, string nuevoEstado)
        {
            var propuesta = await GetByIdAsync(id);
            if (propuesta == null)
                return null;

            propuesta.Estado = nuevoEstado;
            propuesta.FechaActualizacion = FechaEcuador.Ahora();

            _context.Propuestas.Update(propuesta);
            await SaveChangesAsync();
            return propuesta;
        }

        public async Task<IEnumerable<Propuesta>> GetPropuestasPorEstadoAsync(string estado)
        {
            return await _context.Propuestas
                .Where(p => p.Estado == estado)
                .Include(p => p.Profesor)
                .Include(p => p.PropuestasAsignaturas)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
        }
    }
}
