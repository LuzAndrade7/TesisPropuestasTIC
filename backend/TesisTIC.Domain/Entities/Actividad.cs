namespace TesisTIC.Domain.Entities;

/// <summary>
/// Entidad que representa una actividad dentro de un componente
/// Mapea la tabla public.actividades
/// </summary>
public class Actividad
{
    public int Id { get; set; }

    /// <summary>
    /// ID del componente al que pertenece esta actividad
    /// </summary>
    public int ComponenteId { get; set; }

    /// <summary>
    /// Número secuencial de la actividad
    /// </summary>
    public int Numero { get; set; }

    /// <summary>
    /// Descripción de la actividad
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Horas estimadas para completar la actividad
    /// </summary>
    public int Horas { get; set; }

    // Relaciones
    /// <summary>
    /// Componente al que pertenece esta actividad
    /// </summary>
    public virtual Componente Componente { get; set; } = null!;
}
