namespace TesisTIC.Application.DTOs;

/// <summary>
/// DTO para mostrar información de una asignatura
/// </summary>
public class AsignaturaDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}

/// <summary>
/// DTO para crear o actualizar una asignatura
/// </summary>
public class CreateUpdateAsignaturaDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}
