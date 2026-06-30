namespace TesisTIC.Application.DTOs;

/// <summary>
/// DTO para mostrar información de una actividad
/// </summary>
public class ActividadDto
{
    public int Id { get; set; }
    public int ComponenteId { get; set; }
    public int Numero { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int Horas { get; set; }
}

/// <summary>
/// DTO para crear una actividad
/// </summary>
public class CreateActividadDto
{
    public int ComponenteId { get; set; }
    public int Numero { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int Horas { get; set; }
}

/// <summary>
/// DTO para actualizar una actividad
/// </summary>
public class UpdateActividadDto
{
    public int? Numero { get; set; }
    public string? Descripcion { get; set; }
    public int? Horas { get; set; }
}
