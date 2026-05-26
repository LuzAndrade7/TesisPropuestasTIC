using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.Interfaces;
using TesisTIC.Infrastructure.Data;

namespace TesisTIC.Infrastructure.Repositories;

/// <summary>
/// Implementación genérica de repositorio para todas las entidades
/// Proporciona operaciones CRUD estándar
/// </summary>
/// <typeparam name="T">Tipo de entidad</typeparam>
public class GenericRepository<T> : IRepository<T> where T : class
{
    protected readonly TesisTicDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(TesisTicDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
    }

    /// <summary>
    /// Obtiene todas las entidades de forma asincrónica
    /// </summary>
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    /// <summary>
    /// Obtiene una entidad por ID
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// Obtiene el conteo total de entidades
    /// </summary>
    public virtual async Task<int> GetCountAsync()
    {
        return await _dbSet.CountAsync();
    }

    /// <summary>
    /// Crea una nueva entidad
    /// </summary>
    public virtual async Task<T> CreateAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _dbSet.AddAsync(entity);
        await SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Actualiza una entidad existente
    /// </summary>
    public virtual async Task<T> UpdateAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Update(entity);
        await SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Elimina una entidad por ID
    /// </summary>
    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;

        _dbSet.Remove(entity);
        await SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Guarda los cambios en la base de datos
    /// </summary>
    public virtual async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
