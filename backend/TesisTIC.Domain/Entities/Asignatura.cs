namespace TesisTIC.Domain.Entities;

/// <summary>
/// Entidad que representa una asignatura de la carrera
/// Mapea la tabla public.asignaturas
/// </summary>
public class Asignatura
{
    public int Id { get; set; }

    /// <summary>
    /// Código único de la asignatura (Ej: ISTG2001)
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de la asignatura
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    // Relaciones
    /// <summary>
    /// Propuestas asociadas a esta asignatura
    /// </summary>
    public ICollection<PropuestaAsignatura> PropuestasAsignaturas { get; set; } = new List<PropuestaAsignatura>();
}
