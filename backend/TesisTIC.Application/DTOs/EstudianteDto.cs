namespace TesisTIC.Application.DTOs;

/// <summary>
/// DTO para mostrar información de un estudiante
/// </summary>
public class EstudianteDto
{
    public int Id { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string? Correo { get; set; }
    public DateTime FechaCreacion { get; set; }
}

/// <summary>
/// DTO para crear o actualizar un estudiante
/// </summary>
public class CreateUpdateEstudianteDto
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string? Correo { get; set; }
}
