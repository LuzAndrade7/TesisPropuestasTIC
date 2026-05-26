namespace TesisTIC.Application.DTOs;

/// <summary>
/// DTO para mostrar una observación de CPGIC
/// </summary>
public class ObservacionesCpgicDto
{
    public int Id { get; set; }
    public int PropuestaId { get; set; }
    public string Observacion { get; set; } = string.Empty;
    public string? RealizadoPor { get; set; }
    public DateTime FechaObservacion { get; set; }
}

/// <summary>
/// DTO para crear una observación de CPGIC
/// </summary>
public class CreateObservacionesCpgicDto
{
    public int PropuestaId { get; set; }
    public string Observacion { get; set; } = string.Empty;
    public string? RealizadoPor { get; set; }
}
