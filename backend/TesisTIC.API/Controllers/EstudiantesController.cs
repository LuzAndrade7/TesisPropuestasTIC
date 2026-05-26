using Microsoft.AspNetCore.Mvc;
using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;

namespace TesisTIC.API.Controllers;

/// <summary>
/// Controller para operaciones REST de Estudiantes
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EstudiantesController : ControllerBase
{
    private readonly IEstudianteService _service;
    private readonly IPropuestaEstudianteService _propuestaEstudianteService; // HU07 T20
    private readonly ILogger<EstudiantesController> _logger;

    public EstudiantesController(
        IEstudianteService service,
        IPropuestaEstudianteService propuestaEstudianteService, // HU07 T20
        ILogger<EstudiantesController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _propuestaEstudianteService = propuestaEstudianteService ?? throw new ArgumentNullException(nameof(propuestaEstudianteService)); // HU07 T20
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtiene todos los estudiantes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EstudianteDto>>> GetAll()
    {
        try
        {
            var estudiantes = await _service.GetAllAsync();
            return Ok(estudiantes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estudiantes");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene un estudiante por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstudianteDto>> GetById(int id)
    {
        try
        {
            var estudiante = await _service.GetByIdAsync(id);
            if (estudiante == null)
                return NotFound(new { message = $"Estudiante con ID {id} no encontrado" });

            return Ok(estudiante);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estudiante {estudianteId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crea un nuevo estudiante
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EstudianteDto>> Create([FromBody] CreateUpdateEstudianteDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var estudiante = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = estudiante.Id }, estudiante);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear estudiante");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza un estudiante existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EstudianteDto>> Update(int id, [FromBody] CreateUpdateEstudianteDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var estudiante = await _service.UpdateAsync(id, dto);
            return Ok(estudiante);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estudiante {estudianteId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Elimina un estudiante
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var resultado = await _service.DeleteAsync(id);
            if (!resultado)
                return NotFound(new { message = $"Estudiante con ID {id} no encontrado" });

            return Ok(new { message = "Estudiante eliminado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar estudiante {estudianteId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // ===== HU07 T20-T22: ENDPOINTS DE ASIGNACIÓN DE ESTUDIANTES A PROPUESTAS =====

    /// <summary>
    /// HU07 T21: Busca estudiantes por nombre, apellido o correo
    /// </summary>
    [HttpGet("buscar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<EstudianteDto>>> Buscar([FromQuery] string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest(new { message = "Término de búsqueda no puede estar vacío" });

            var estudiantes = await _propuestaEstudianteService.BuscarEstudiantesAsync(searchTerm);
            _logger.LogInformation("✅ HU07 T21: Búsqueda '{searchTerm}': {count} resultados", searchTerm, estudiantes.Count);
            return Ok(estudiantes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar estudiantes");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// HU07 T21: Obtiene estudiantes disponibles (no asignados a una propuesta específica)
    /// </summary>
    [HttpGet("disponibles/{propuestaId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<EstudianteDto>>> GetDisponibles(int propuestaId)
    {
        try
        {
            var estudiantes = await _propuestaEstudianteService.ObtenerEstudiantesDisponiblesAsync(propuestaId);
            return Ok(estudiantes);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estudiantes disponibles");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// HU07 T20: Obtiene estudiantes asignados a una propuesta
    /// </summary>
    [HttpGet("asignados/{propuestaId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<PropuestaEstudianteDto>>> GetAsignados(int propuestaId)
    {
        try
        {
            var estudiantes = await _propuestaEstudianteService.ObtenerEstudiantesPorPropuestaAsync(propuestaId);
            return Ok(estudiantes);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estudiantes asignados");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// HU07 T20: Asigna estudiantes a una propuesta (máximo 5)
    /// </summary>
    [HttpPost("{propuestaId}/asignar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PropuestaEstudianteDto>>> Asignar(
        int propuestaId,
        [FromBody] AsignarEstudiantesRequest request)
    {
        try
        {
            if (request == null || request.EstudianteIds == null || request.EstudianteIds.Count == 0)
                return BadRequest(new { message = "Debe proporcionar al menos un estudiante" });

            if (request.EstudianteIds.Count > 5)
                return BadRequest(new { message = "No se pueden asignar más de 5 estudiantes por propuesta" });

            var asignaciones = await _propuestaEstudianteService.AsignarEstudiantesAsync(
                propuestaId,
                request.EstudianteIds,
                request.Motivo,
                request.RealizadoPor);

            _logger.LogInformation("✅ HU07 T20: Asignados {count} estudiantes a propuesta {propuestaId}",
                asignaciones.Count, propuestaId);
            return Ok(asignaciones);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar estudiantes");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
