namespace TesisTIC.Domain.Entities;

/// <summary>
/// Entidad que representa un componente o módulo de una propuesta
/// Mapea la tabla public.componentes
/// </summary>
public class Componente
{
    public int Id { get; set; }

    /// <summary>
    /// ID de la propuesta a la que pertenece este componente
    /// </summary>
    public int PropuestaId { get; set; }

    /// <summary>
    /// ID del estudiante asignado a este componente (opcional)
    /// </summary>
    public int? EstudianteId { get; set; }

    /// <summary>
    /// Nombre del componente/módulo
    /// </summary>
    public string? Nombre { get; set; }

    /// <summary>
    /// Descripción detallada del componente
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Orden de aparición del componente dentro de la propuesta
    /// </summary>
    public int Orden { get; set; }

    // Relaciones
    /// <summary>
    /// Propuesta a la que pertenece este componente
    /// </summary>
    public virtual Propuesta Propuesta { get; set; } = null!;

    /// <summary>
    /// Estudiante asignado a este componente
    /// </summary>
    public virtual Estudiante? Estudiante { get; set; }

    /// <summary>
    /// Actividades de este componente
    /// </summary>
    public ICollection<Actividad> Actividades { get; set; } = new List<Actividad>();

    /// <summary>
    /// Productos esperados de este componente
    /// </summary>
    public ICollection<ProductoEsperado> ProductosEsperados { get; set; } = new List<ProductoEsperado>();
}
