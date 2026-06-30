namespace TesisTIC.Domain.Entities;

/// <summary>
/// Entidad que representa observaciones realizadas por la CPGIC (Comisión Permanente de Gestión Integral del Currículo)
/// Mapea la tabla public.observaciones_cpgic
/// </summary>
public class ObservacionesCpgic
{
    public int Id { get; set; }

    /// <summary>
    /// ID de la propuesta observada
    /// </summary>
    public int PropuestaId { get; set; }

    /// <summary>
    /// Texto de la observación
    /// </summary>
    public string Observacion { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de la persona que realizó la observación
    /// </summary>
    public string? RealizadoPor { get; set; }

    /// <summary>
    /// Fecha de la observación
    /// </summary>
    public DateTime FechaObservacion { get; set; } = DateTime.UtcNow;

    // Relaciones
    /// <summary>
    /// Propuesta que fue observada
    /// </summary>
    public virtual Propuesta Propuesta { get; set; } = null!;
}
