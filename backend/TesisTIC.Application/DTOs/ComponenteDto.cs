namespace TesisTIC.Application.DTOs;

/// <summary>
/// DTO para mostrar información de un componente
/// </summary>
public class ComponenteDto
{
    public int Id { get; set; }
    public int PropuestaId { get; set; }
    public int? EstudianteId { get; set; }
    public string? Nombre { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int Orden { get; set; }

    // Relaciones
    public EstudianteDto? Estudiante { get; set; }
    public List<ActividadDto> Actividades { get; set; } = new();
    public List<ProductoEsperadoDto> ProductosEsperados { get; set; } = new();
}

/// <summary>
/// DTO para crear un componente
/// </summary>
public class CreateComponenteDto
{
    public int PropuestaId { get; set; }
    public int? EstudianteId { get; set; }
    public string? Nombre { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int Orden { get; set; }
}

/// <summary>
/// DTO para actualizar un componente
/// </summary>
public class UpdateComponenteDto
{
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public int? Orden { get; set; }
    public int? EstudianteId { get; set; }
}
