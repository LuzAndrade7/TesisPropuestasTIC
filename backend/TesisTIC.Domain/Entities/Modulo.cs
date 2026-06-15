namespace TesisTIC.Domain.Entities;

/// <summary>
/// Entidad que representa un módulo dentro de una propuesta
/// </summary>
public class Modulo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    public int PropuestaId { get; set; }
    public virtual Propuesta Propuesta { get; set; } = null!;
}
