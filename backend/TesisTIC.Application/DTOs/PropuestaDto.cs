namespace TesisTIC.Application.DTOs;

/// <summary>
/// DTO para mostrar información de una propuesta con relaciones
/// </summary>
public class PropuestaDto
{
    public int Id { get; set; }
    public string NombreProyecto { get; set; } = string.Empty;
    public int NumeroParticipantes { get; set; }
    public int ProfesorId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Objetivo { get; set; } = string.Empty;
    public string Alcance { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
    public DateTime? FechaEnvioRevision { get; set; }

    // Relaciones
    public DocenteDto? Profesor { get; set; }
    public List<AsignaturaDto> Asignaturas { get; set; } = new();
}

/// <summary>
/// DTO para crear una propuesta
/// </summary>
public class CreatePropuestaDto
{
    public string NombreProyecto { get; set; } = string.Empty;
    public int NumeroParticipantes { get; set; }
    public int ProfesorId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Objetivo { get; set; } = string.Empty;
    public string Alcance { get; set; } = string.Empty;
    public List<int>? AsignaturaIds { get; set; } = new();
}

/// <summary>
/// DTO para actualizar una propuesta
/// </summary>
public class UpdatePropuestaDto
{
    public string? NombreProyecto { get; set; }
    public int? NumeroParticipantes { get; set; }
    public string? Descripcion { get; set; }
    public string? Objetivo { get; set; }
    public string? Alcance { get; set; }
    public List<int>? AsignaturaIds { get; set; }
}

/// <summary>
/// DTO para cambiar el estado de una propuesta
/// </summary>
public class UpdateEstadoPropuestaDto
{
    public string Estado { get; set; } = string.Empty;
}
