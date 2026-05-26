using Microsoft.AspNetCore.Mvc;
using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;

namespace TesisTIC.API.Controllers;

/// <summary>
/// Controller para operaciones REST de Asignaturas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AsignaturasController : ControllerBase
{
    private readonly IAsignaturaService _service;
    private readonly ILogger<AsignaturasController> _logger;

    public AsignaturasController(
        IAsignaturaService service,
        ILogger<AsignaturasController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtiene todas las asignaturas
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AsignaturaDto>>> GetAll()
    {
        try
        {
            var asignaturas = await _service.GetAllAsync();
            return Ok(asignaturas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asignaturas");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene una asignatura por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AsignaturaDto>> GetById(int id)
    {
        try
        {
            var asignatura = await _service.GetByIdAsync(id);
            if (asignatura == null)
                return NotFound(new { message = $"Asignatura con ID {id} no encontrada" });

            return Ok(asignatura);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asignatura {asignaturaId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crea una nueva asignatura
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AsignaturaDto>> Create([FromBody] CreateUpdateAsignaturaDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var asignatura = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = asignatura.Id }, asignatura);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear asignatura");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza una asignatura existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AsignaturaDto>> Update(int id, [FromBody] CreateUpdateAsignaturaDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var asignatura = await _service.UpdateAsync(id, dto);
            return Ok(asignatura);
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
            _logger.LogError(ex, "Error al actualizar asignatura {asignaturaId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Elimina una asignatura
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
                return NotFound(new { message = $"Asignatura con ID {id} no encontrada" });

            return Ok(new { message = "Asignatura eliminada exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar asignatura {asignaturaId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
