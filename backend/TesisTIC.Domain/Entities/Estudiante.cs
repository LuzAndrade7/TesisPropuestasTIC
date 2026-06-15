namespace TesisTIC.Domain.Entities;

/// <summary>
/// Entidad que representa un estudiante que puede asignarse a un componente.
/// </summary>
public class Estudiante
{
    public int Id { get; set; }

    /// <summary>
    /// Nombre completo del estudiante.
    /// </summary>
    public string NombresEstudiante { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de creacion del registro.
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Componentes asignados a este estudiante.
    /// </summary>
    public ICollection<Componente> Componentes { get; set; } = new List<Componente>();

    /// <summary>
    /// Relacion historica del flujo anterior de asignacion por propuesta.
    /// </summary>
    public ICollection<PropuestaEstudiante> PropuestaEstudiantes { get; set; } = new List<PropuestaEstudiante>();
}
