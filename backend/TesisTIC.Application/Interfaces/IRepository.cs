namespace TesisTIC.Application.Interfaces;

/// <summary>
/// Interfaz genérica para operaciones de lectura en repositorios
/// </summary>
/// <typeparam name="T">Tipo de entidad</typeparam>
public interface IReadRepository<T> where T : class
{
    /// <summary>
    /// Obtiene todas las entidades de forma asincrónica
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Obtiene una entidad por ID
    /// </summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene el conteo total de entidades
    /// </summary>
    Task<int> GetCountAsync();
}

/// <summary>
/// Interfaz genérica para operaciones de escritura en repositorios
/// </summary>
/// <typeparam name="T">Tipo de entidad</typeparam>
public interface IWriteRepository<T> where T : class
{
    /// <summary>
    /// Crea una nueva entidad
    /// </summary>
    Task<T> CreateAsync(T entity);

    /// <summary>
    /// Actualiza una entidad existente
    /// </summary>
    Task<T> UpdateAsync(T entity);

    /// <summary>
    /// Elimina una entidad por ID
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Guarda los cambios en la base de datos
    /// </summary>
    Task<int> SaveChangesAsync();
}

/// <summary>
/// Interfaz combinada para operaciones CRUD completas
/// </summary>
/// <typeparam name="T">Tipo de entidad</typeparam>
public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T> where T : class
{
}
