namespace TesisTIC.Domain.Entities;

/// <summary>
/// Entidad que representa las observaciones realizadas a una propuesta
/// </summary>
public class ObservacionPropuesta
{
    public int Id { get; set; }
    public string Contenido { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public int PropuestaId { get; set; }
    public virtual Propuesta Propuesta { get; set; } = null!;
}
