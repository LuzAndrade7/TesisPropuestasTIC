namespace TesisTIC.Domain.Entities;

/// <summary>
/// Entidad que representa un estudiante que participa en propuestas
/// Mapea la tabla public.estudiantes
/// </summary>
public class Estudiante
{
    public int Id { get; set; }

    /// <summary>
    /// Nombres del estudiante
    /// </summary>
    public string Nombres { get; set; } = string.Empty;

    /// <summary>
    /// Apellidos del estudiante
    /// </summary>
    public string Apellidos { get; set; } = string.Empty;

    /// <summary>
    /// Email del estudiante
    /// </summary>
    public string? Correo { get; set; }

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Relaciones
    /// <summary>
    /// Componentes asignados a este estudiante
    /// </summary>
    public ICollection<Componente> Componentes { get; set; } = new List<Componente>();

    /// <summary>
    /// HU07 T20: Propuestas a las que está asignado este estudiante
    /// </summary>
    public ICollection<PropuestaEstudiante> PropuestaEstudiantes { get; set; } = new List<PropuestaEstudiante>();
}
