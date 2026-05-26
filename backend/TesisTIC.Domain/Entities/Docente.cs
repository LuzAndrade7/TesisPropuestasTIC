namespace TesisTIC.Domain.Entities;

/// <summary>
/// Entidad que representa un docente de la Escuela Politécnica Nacional
/// Mapea la tabla public.docentes
/// </summary>
public class Docente
{
    public int Id { get; set; }

    /// <summary>
    /// Nombres del docente
    /// </summary>
    public string Nombres { get; set; } = string.Empty;

    /// <summary>
    /// Apellidos del docente
    /// </summary>
    public string Apellidos { get; set; } = string.Empty;

    /// <summary>
    /// Email de contacto del docente
    /// </summary>
    public string? Correo { get; set; }

    /// <summary>
    /// Título académico del docente (Ej: Ingeniero, Master, Doctor)
    /// </summary>
    public string? TituloAcademico { get; set; }

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Relaciones
    /// <summary>
    /// Propuestas que ha creado este docente
    /// </summary>
    public ICollection<Propuesta> Propuestas { get; set; } = new List<Propuesta>();
}
