using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;
using TesisTIC.Infrastructure.Data;

namespace TesisTIC.Infrastructure.Repositories;

/// <summary>
/// Repositorio específico para operaciones con Propuestas
/// Extiende el repositorio genérico con consultas personalizadas
/// </summary>
public class PropuestaRepository : GenericRepository<Propuesta>, IPropuestaRepository
{
    public PropuestaRepository(TesisTicDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtiene propuestas con filtros opcionales (estado, profesor)
    /// </summary>
    public async Task<IEnumerable<Propuesta>> GetPropuestasAsync(
        string? estado = null,
        int? profesorId = null)
    {
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(estado))
            query = query.Where(p => p.Estado == estado);

        if (profesorId.HasValue)
            query = query.Where(p => p.ProfesorId == profesorId);

        return await query
            .Include(p => p.Profesor)
            .Include(p => p.PropuestasAsignaturas)
            .ThenInclude(pa => pa.Asignatura)
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene una propuesta con todas sus relaciones cargadas
    /// </summary>
    public async Task<Propuesta?> GetPropuestaFullAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Profesor)
            .Include(p => p.PropuestasAsignaturas)
            .ThenInclude(pa => pa.Asignatura)
            .Include(p => p.Componentes)
            .ThenInclude(c => c.Actividades)
            .Include(p => p.Componentes)
            .ThenInclude(c => c.ProductosEsperados)
            .Include(p => p.Componentes)
            .ThenInclude(c => c.Estudiante)
            .Include(p => p.Observaciones)
            .Include(p => p.AprobacionCpgic)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Obtiene todas las propuestas de un profesor específico
    /// </summary>
    public async Task<IEnumerable<Propuesta>> GetPropuestasPorProfesorAsync(int profesorId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.ProfesorId == profesorId)
            .Include(p => p.PropuestasAsignaturas)
            .ThenInclude(pa => pa.Asignatura)
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync();
    }

    /// <summary>
    /// Actualiza una propuesta existente y recarga las relaciones
    /// </summary>
    public override async Task<Propuesta> UpdateAsync(Propuesta entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Update(entity);
        await SaveChangesAsync();

        // Recargar con todas las relaciones después de actualizar
        return await GetPropuestaFullAsync(entity.Id) ?? entity;
    }

    /// <summary>
    /// Crea una propuesta existente y recarga las relaciones
    /// </summary>
    public override async Task<Propuesta> CreateAsync(Propuesta entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _dbSet.AddAsync(entity);
        await SaveChangesAsync();

        // Recargar con todas las relaciones después de crear
        return await GetPropuestaFullAsync(entity.Id) ?? entity;
    }

    /// <summary>
    /// Cambia el estado de una propuesta
    /// </summary>
    public async Task<Propuesta?> CambiarEstadoAsync(int id, string nuevoEstado)
    {
        var propuesta = await _dbSet.FindAsync(id);
        if (propuesta == null)
            return null;

        // Validar que el nuevo estado sea válido
        var estadosValidos = new[] { "BORRADOR", "PENDIENTE", "OBSERVADA", "APROBADA", "RECHAZADA" };
        if (!estadosValidos.Contains(nuevoEstado))
            throw new ArgumentException($"Estado inválido: {nuevoEstado}");

        propuesta.Estado = nuevoEstado;
        propuesta.FechaActualizacion = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        if (nuevoEstado == "PENDIENTE")
            propuesta.FechaEnvioRevision = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        _dbSet.Update(propuesta);
        await SaveChangesAsync();
        return propuesta;
    }

    /// <summary>
    /// Obtiene propuestas filtradas por estado
    /// </summary>
    public async Task<IEnumerable<Propuesta>> GetPropuestasPorEstadoAsync(string estado)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.Estado == estado)
            .Include(p => p.Profesor)
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync();
    }
}
