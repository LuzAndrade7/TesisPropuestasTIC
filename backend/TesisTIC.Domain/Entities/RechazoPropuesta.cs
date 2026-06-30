namespace TesisTIC.Domain.Entities;

/// <summary>
/// Entidad que representa el rechazo de una propuesta
/// </summary>
public class RechazoPropuesta
{
    public int Id { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public DateTime FechaRechazo { get; set; } = DateTime.UtcNow;

    public int PropuestaId { get; set; }
    public virtual Propuesta Propuesta { get; set; } = null!;
}
