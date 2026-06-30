namespace TesisTIC.Domain.Entities;

/// <summary>
/// Entidad de relación entre Propuestas y Asignaturas (Many-to-Many)
/// Mapea la tabla public.propuesta_asignaturas
/// </summary>
public class PropuestaAsignatura
{
    public int Id { get; set; }

    /// <summary>
    /// ID de la propuesta
    /// </summary>
    public int PropuestaId { get; set; }

    /// <summary>
    /// ID de la asignatura
    /// </summary>
    public int AsignaturaId { get; set; }

    // Relaciones
    /// <summary>
    /// Propuesta relacionada
    /// </summary>
    public virtual Propuesta Propuesta { get; set; } = null!;

    /// <summary>
    /// Asignatura relacionada
    /// </summary>
    public virtual Asignatura Asignatura { get; set; } = null!;
}
