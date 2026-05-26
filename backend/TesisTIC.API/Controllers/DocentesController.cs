using Microsoft.AspNetCore.Mvc;
using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;

namespace TesisTIC.API.Controllers;

/// <summary>
/// Controller para operaciones REST de Docentes
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DocentesController : ControllerBase
{
    private readonly IDocenteService _service;
    private readonly ILogger<DocentesController> _logger;

    public DocentesController(
        IDocenteService service,
        ILogger<DocentesController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtiene todos los docentes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DocenteDto>>> GetAll()
    {
        try
        {
            var docentes = await _service.GetAllAsync();
            return Ok(docentes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener docentes");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene un docente por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DocenteDto>> GetById(int id)
    {
        try
        {
            var docente = await _service.GetByIdAsync(id);
            if (docente == null)
                return NotFound(new { message = $"Docente con ID {id} no encontrado" });

            return Ok(docente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener docente {docenteId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crea un nuevo docente
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DocenteDto>> Create([FromBody] CreateUpdateDocenteDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var docente = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = docente.Id }, docente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear docente");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza un docente existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DocenteDto>> Update(int id, [FromBody] CreateUpdateDocenteDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var docente = await _service.UpdateAsync(id, dto);
            return Ok(docente);
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
            _logger.LogError(ex, "Error al actualizar docente {docenteId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Elimina un docente
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
                return NotFound(new { message = $"Docente con ID {id} no encontrado" });

            return Ok(new { message = "Docente eliminado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar docente {docenteId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
