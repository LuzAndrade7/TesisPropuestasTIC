namespace TesisTIC.Domain.Entities;

/// <summary>
/// Entidad que representa la aprobación final de una propuesta por la CPGIC
/// Mapea la tabla public.aprobaciones_cpgic
/// </summary>
public class AprobacionesCpgic
{
    public int Id { get; set; }

    /// <summary>
    /// ID único de la propuesta aprobada
    /// </summary>
    public int PropuestaId { get; set; }

    /// <summary>
    /// Número de resolución de aprobación
    /// </summary>
    public string? Resolucion { get; set; }

    /// <summary>
    /// Nombre del presidente de la CPGIC
    /// </summary>
    public string? PresidenteCpgic { get; set; }

    /// <summary>
    /// Fecha de la aprobación
    /// </summary>
    public DateTime? FechaAprobacion { get; set; }

    /// <summary>
    /// Firma digital del director (base64 o path)
    /// </summary>
    public string? FirmaDirector { get; set; }

    /// <summary>
    /// Firma digital del presidente (base64 o path)
    /// </summary>
    public string? FirmaPresidente { get; set; }

    /// <summary>
    /// Estado de la aprobación (APROBADA, RECHAZADA, PENDIENTE)
    /// </summary>
    public string? EstadoAprobacion { get; set; }

    // Relaciones
    /// <summary>
    /// Propuesta relacionada
    /// </summary>
    public virtual Propuesta Propuesta { get; set; } = null!;
}
