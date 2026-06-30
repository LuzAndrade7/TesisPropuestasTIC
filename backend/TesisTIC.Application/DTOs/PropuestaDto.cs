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
    public List<ComponenteDto> Componentes { get; set; } = new();
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
    public List<CreateComponenteConDetalleDto>? Componentes { get; set; } = new();
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
    public List<CreateComponenteConDetalleDto>? Componentes { get; set; }
}

/// <summary>
/// DTO para guardar un componente con sus actividades y productos desde el formulario de propuesta
/// </summary>
public class CreateComponenteConDetalleDto
{
    public string? Nombre { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int Orden { get; set; }
    public string? NombresEstudiante { get; set; }
    public List<CreateActividadSimpleDto> Actividades { get; set; } = new();
    public List<CreateProductoSimpleDto> ProductosEsperados { get; set; } = new();
}

/// <summary>
/// DTO simple de actividad anidada en una propuesta
/// </summary>
public class CreateActividadSimpleDto
{
    public int Numero { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int Horas { get; set; }
}

/// <summary>
/// DTO simple de producto esperado anidado en una propuesta
/// </summary>
public class CreateProductoSimpleDto
{
    public string Descripcion { get; set; } = string.Empty;
}

/// <summary>
/// DTO para cambiar el estado de una propuesta
/// </summary>
public class UpdateEstadoPropuestaDto
{
    public string Estado { get; set; } = string.Empty;
}

/// <summary>
/// DTO para solicitar una nueva revision de una propuesta que ya fue aprobada.
/// </summary>
public class SolicitarNuevaAprobacionDto
{
    public string Motivo { get; set; } = string.Empty;
}
