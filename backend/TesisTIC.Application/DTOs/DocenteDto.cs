namespace TesisTIC.Application.DTOs;

/// <summary>
/// DTO para mostrar información básica de un docente
/// </summary>
public class DocenteDto
{
    public int Id { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string? Correo { get; set; }
    public string? TituloAcademico { get; set; }
    public DateTime FechaCreacion { get; set; }
}

/// <summary>
/// DTO para crear o actualizar un docente
/// </summary>
public class CreateUpdateDocenteDto
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string? Correo { get; set; }
    public string? TituloAcademico { get; set; }
}
