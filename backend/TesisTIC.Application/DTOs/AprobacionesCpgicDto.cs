namespace TesisTIC.Application.DTOs;

/// <summary>
/// DTO para mostrar información de aprobación CPGIC
/// </summary>
public class AprobacionesCpgicDto
{
    public int Id { get; set; }
    public int PropuestaId { get; set; }
    public string? Resolucion { get; set; }
    public string? PresidenteCpgic { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? EstadoAprobacion { get; set; }
}

/// <summary>
/// DTO para crear aprobación CPGIC
/// </summary>
public class CreateAprobacionesCpgicDto
{
    public int PropuestaId { get; set; }
    public string? Resolucion { get; set; }
    public string? PresidenteCpgic { get; set; }
    public string? EstadoAprobacion { get; set; }
}

/// <summary>
/// DTO para actualizar aprobación CPGIC
/// </summary>
public class UpdateAprobacionesCpgicDto
{
    public string? Resolucion { get; set; }
    public string? PresidenteCpgic { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? EstadoAprobacion { get; set; }
}
