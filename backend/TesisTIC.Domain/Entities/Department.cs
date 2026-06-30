namespace TesisTIC.Domain.Entities;

/// <summary>
/// Entidad que representa un departamento académico
/// </summary>
public class Department
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    public virtual ICollection<Propuesta> Propuestas { get; set; } = new List<Propuesta>();
}
