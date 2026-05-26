using Microsoft.AspNetCore.Mvc;
using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;

namespace TesisTIC.API.Controllers;

/// <summary>
/// HU04 T10: Controller para operaciones de observaciones CPGIC
/// Endpoints para registrar, listar y limpiar observaciones sobre propuestas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ObservacionesController : ControllerBase
{
    private readonly IObservacionesCpgicService _service;
    private readonly ILogger<ObservacionesController> _logger;

    public ObservacionesController(
        IObservacionesCpgicService service,
        ILogger<ObservacionesController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// HU04 T10: Registra una nueva observación sobre una propuesta
    /// Cambia automáticamente el estado de la propuesta a OBSERVADA
    /// </summary>
    /// <param name="dto">Datos de la observación (propuestaId, observacion, realizadoPor)</param>
    /// <returns>Observación creada</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ObservacionesCpgicDto>> CrearObservacion([FromBody] CreateObservacionesCpgicDto dto)
    {
        try
        {
            var observacion = await _service.CrearObservacionAsync(dto);
            _logger.LogInformation("✅ Observación creada para propuesta {propuestaId}", dto.PropuestaId);
            return CreatedAtAction(nameof(ObtenerObservacion), new { id = observacion.Id }, observacion);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Error de validación: {mensaje}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("⚠️ Operación inválida: {mensaje}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear observación");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// HU04 T10: Obtiene todas las observaciones de una propuesta
    /// Ordenadas por fecha descendente (más recientes primero)
    /// </summary>
    /// <param name="propuestaId">ID de la propuesta</param>
    /// <returns>Lista de observaciones</returns>
    [HttpGet("propuesta/{propuestaId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ObservacionesCpgicDto>>> ObtenerObservacionesPropuesta(int propuestaId)
    {
        try
        {
            var observaciones = await _service.ObtenerObservacionesPorPropuestaAsync(propuestaId);
            _logger.LogInformation("✅ Observaciones obtenidas para propuesta {propuestaId}", propuestaId);
            return Ok(observaciones);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Error de validación: {mensaje}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener observaciones");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// HU04 T10: Obtiene una observación específica
    /// </summary>
    /// <param name="id">ID de la observación</param>
    /// <returns>Observación encontrada</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ObservacionesCpgicDto>> ObtenerObservacion(int id)
    {
        try
        {
            var observacion = await _service.ObtenerObservacionAsync(id);
            if (observacion == null)
            {
                return NotFound(new { message = $"Observación con ID {id} no encontrada" });
            }

            return Ok(observacion);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener observación {id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// HU04 T10: Elimina una observación específica
    /// </summary>
    /// <param name="id">ID de la observación</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> EliminarObservacion(int id)
    {
        try
        {
            var resultado = await _service.EliminarObservacionAsync(id);
            if (!resultado)
            {
                return NotFound(new { message = $"Observación con ID {id} no encontrada" });
            }

            _logger.LogInformation("✅ Observación {id} eliminada", id);
            return Ok(new { message = "Observación eliminada exitosamente" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar observación {id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// HU04 T10: Limpia TODAS las observaciones de una propuesta
    /// Se usa cuando se reenvía la propuesta después de correcciones
    /// Cambia estado OBSERVADA → PENDIENTE
    /// </summary>
    /// <param name="propuestaId">ID de la propuesta</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("propuesta/{propuestaId}/limpiar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> LimpiarObservaciones(int propuestaId)
    {
        try
        {
            var resultado = await _service.LimpiarObservacionesPorPropuestaAsync(propuestaId);
            if (!resultado)
            {
                return Ok(new { message = "La propuesta no tenía observaciones registradas" });
            }

            _logger.LogInformation("✅ Observaciones limpias para propuesta {propuestaId}", propuestaId);
            return Ok(new { message = "Observaciones eliminadas exitosamente" });
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
            _logger.LogError(ex, "Error al limpiar observaciones de propuesta {propuestaId}", propuestaId);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// HU04 T10: Verifica si una propuesta tiene observaciones
    /// </summary>
    /// <param name="propuestaId">ID de la propuesta</param>
    /// <returns>true si tiene observaciones, false si no</returns>
    [HttpGet("propuesta/{propuestaId}/tiene-observaciones")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<bool>> TieneObservaciones(int propuestaId)
    {
        try
        {
            var resultado = await _service.TieneObservacionesAsync(propuestaId);
            return Ok(resultado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verificando observaciones de propuesta {propuestaId}", propuestaId);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// HU04 T10: Cuenta observaciones de una propuesta
    /// </summary>
    /// <param name="propuestaId">ID de la propuesta</param>
    /// <returns>Número de observaciones</returns>
    [HttpGet("propuesta/{propuestaId}/contar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> ContarObservaciones(int propuestaId)
    {
        try
        {
            var cantidad = await _service.ContarObservacionesAsync(propuestaId);
            return Ok(new { cantidad = cantidad });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error contando observaciones de propuesta {propuestaId}", propuestaId);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
