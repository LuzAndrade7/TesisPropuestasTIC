using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;
using TesisTIC.Infrastructure.Persistence;

namespace TesisTIC.Infrastructure.Repositories
{
    public class PropuestaRepository : IPropuestaRepository
    {
        private readonly TesisTICDbContext _context;

        public PropuestaRepository(TesisTICDbContext context)
        {
            _context = context;
        }

        public async Task<Propuesta> ObtenerPorIdAsync(int id)
        {
            return await _context.Propuestas
                .Include(p => p.Docente)
                .Include(p => p.Estado)
                .Include(p => p.LineaInvestigacion)
                .Include(p => p.EstudiantesAsignados)
                .ThenInclude(pe => pe.Estudiante)
                .Include(p => p.AsignaturasAsignadas)
                .ThenInclude(pa => pa.Asignatura)
                .Include(p => p.Modulos)
                .ThenInclude(m => m.Actividades)
                .Include(p => p.Modulos)
                .ThenInclude(m => m.Observaciones)
                .Include(p => p.Observaciones)
                .Include(p => p.Rechazo)
                .ThenInclude(r => r.Razones)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Propuesta>> ObtenerTodasAsync()
        {
            return await _context.Propuestas
                .Include(p => p.Docente)
                .Include(p => p.Estado)
                .Include(p => p.LineaInvestigacion)
                .Include(p => p.EstudiantesAsignados)
                .ThenInclude(pe => pe.Estudiante)
                .Include(p => p.AsignaturasAsignadas)
                .ThenInclude(pa => pa.Asignatura)
                .Include(p => p.Modulos)
                .ThenInclude(m => m.Actividades)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Propuesta>> ObtenerPorDocenteAsync(int docenteId)
        {
            return await _context.Propuestas
                .Where(p => p.DocenteId == docenteId)
                .Include(p => p.Docente)
                .Include(p => p.Estado)
                .Include(p => p.LineaInvestigacion)
                .Include(p => p.EstudiantesAsignados)
                .ThenInclude(pe => pe.Estudiante)
                .Include(p => p.AsignaturasAsignadas)
                .ThenInclude(pa => pa.Asignatura)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Propuesta>> ObtenerPorEstadoAsync(int estadoId)
        {
            return await _context.Propuestas
                .Where(p => p.EstadoId == estadoId)
                .Include(p => p.Docente)
                .Include(p => p.Estado)
                .Include(p => p.LineaInvestigacion)
                .Include(p => p.EstudiantesAsignados)
                .ThenInclude(pe => pe.Estudiante)
                .Include(p => p.AsignaturasAsignadas)
                .ThenInclude(pa => pa.Asignatura)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
        }

        public async Task<Propuesta> CrearAsync(Propuesta propuesta)
        {
            propuesta.FechaCreacion = DateTime.UtcNow;
            var resultado = await _context.Propuestas.AddAsync(propuesta);
            await GuardarCambiosAsync();
            return resultado.Entity;
        }

        public async Task<Propuesta> ActualizarAsync(Propuesta propuesta)
        {
            propuesta.FechaActualizacion = DateTime.UtcNow;
            _context.Propuestas.Update(propuesta);
            await GuardarCambiosAsync();
            return propuesta;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var propuesta = await _context.Propuestas.FindAsync(id);
            if (propuesta == null)
                return false;

            _context.Propuestas.Remove(propuesta);
            await GuardarCambiosAsync();
            return true;
        }

        public async Task<int> GuardarCambiosAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
