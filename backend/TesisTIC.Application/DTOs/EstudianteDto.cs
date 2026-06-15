namespace TesisTIC.Application.DTOs;

/// <summary>
/// DTO para mostrar información de un estudiante
/// </summary>
public class EstudianteDto
{
    public int Id { get; set; }
    public string NombresEstudiante { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}

/// <summary>
/// DTO para crear o actualizar un estudiante
/// </summary>
public class CreateUpdateEstudianteDto
{
    public string NombresEstudiante { get; set; } = string.Empty;
}
