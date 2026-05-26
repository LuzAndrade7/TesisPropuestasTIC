namespace TesisTIC.Domain.Entities;

/// <summary>
/// Entidad que representa una propuesta TIC de la Escuela Politécnica Nacional
/// Mapea la tabla public.propuestas
/// </summary>
public class Propuesta
{
    public int Id { get; set; }

    /// <summary>
    /// Nombre o título del proyecto propuesto
    /// </summary>
    public string NombreProyecto { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad de estudiantes que participarán en el proyecto
    /// </summary>
    public int NumeroParticipantes { get; set; }

    /// <summary>
    /// ID del docente que propone el proyecto
    /// </summary>
    public int ProfesorId { get; set; }

    /// <summary>
    /// Descripción detallada de la propuesta
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Objetivo principal del proyecto
    /// </summary>
    public string Objetivo { get; set; } = string.Empty;

    /// <summary>
    /// Alcance del proyecto
    /// </summary>
    public string Alcance { get; set; } = string.Empty;

    /// <summary>
    /// Estado actual de la propuesta: BORRADOR, PENDIENTE, OBSERVADA, APROBADA, RECHAZADA
    /// </summary>
    public string Estado { get; set; } = "BORRADOR";

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Última fecha de actualización del registro
    /// </summary>
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha cuando se envió la propuesta para revisión
    /// </summary>
    public DateTime? FechaEnvioRevision { get; set; }

    // Relaciones
    /// <summary>
    /// Docente que propone el proyecto
    /// </summary>
    public virtual Docente? Profesor { get; set; }

    /// <summary>
    /// Asignaturas asociadas a esta propuesta
    /// </summary>
    public ICollection<PropuestaAsignatura> PropuestasAsignaturas { get; set; } = new List<PropuestaAsignatura>();

    /// <summary>
    /// Componentes (módulos) del proyecto
    /// </summary>
    public ICollection<Componente> Componentes { get; set; } = new List<Componente>();

    /// <summary>
    /// Observaciones realizadas por CPGIC
    /// </summary>
    public ICollection<ObservacionesCpgic> Observaciones { get; set; } = new List<ObservacionesCpgic>();

    /// <summary>
    /// Aprobación de CPGIC
    /// </summary>
    public AprobacionesCpgic? AprobacionCpgic { get; set; }

    /// <summary>
    /// HU07 T20: Estudiantes asignados a esta propuesta
    /// </summary>
    public ICollection<PropuestaEstudiante> PropuestaEstudiantes { get; set; } = new List<PropuestaEstudiante>();
}
