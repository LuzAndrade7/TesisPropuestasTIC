using Microsoft.AspNetCore.Mvc;
using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;

namespace TesisTIC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropuestasController : ControllerBase
    {
        private readonly IPropuestaService _propuestaService;
        private readonly ILogger<PropuestasController> _logger;

        public PropuestasController(IPropuestaService propuestaService, ILogger<PropuestasController> logger)
        {
            _propuestaService = propuestaService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PropuestaDto>> CrearPropuesta([FromBody] CrearPropuestaDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var propuesta = await _propuestaService.CrearPropuestaAsync(dto);
                return CreatedAtAction(nameof(ObtenerPropuesta), new { id = propuesta.Id }, propuesta);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Error de validación al crear propuesta: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear propuesta: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PropuestaDto>> ObtenerPropuesta(int id)
        {
            try
            {
                var propuesta = await _propuestaService.ObtenerPropuestaAsync(id);
                return Ok(propuesta);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Propuesta no encontrada: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener propuesta: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PropuestaDto>>> ObtenerTodas()
        {
            try
            {
                var propuestas = await _propuestaService.ObtenerTodasAsync();
                return Ok(propuestas);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener propuestas: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("docente/{docenteId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PropuestaDto>>> ObtenerPorDocente(int docenteId)
        {
            try
            {
                var propuestas = await _propuestaService.ObtenerPorDocenteAsync(docenteId);
                return Ok(propuestas);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Error al obtener propuestas del docente: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener propuestas por docente: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("estado/{estadoId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PropuestaDto>>> ObtenerPorEstado(int estadoId)
        {
            try
            {
                var propuestas = await _propuestaService.ObtenerPorEstadoAsync(estadoId);
                return Ok(propuestas);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Error al obtener propuestas por estado: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener propuestas por estado: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PropuestaDto>> ActualizarPropuesta(int id, [FromBody] ActualizarPropuestaDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID del objeto." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var propuesta = await _propuestaService.ActualizarPropuestaAsync(dto);
                return Ok(propuesta);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Propuesta no encontrada: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Error de validación al actualizar: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar propuesta: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPatch("{id}/estado")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<object>> CambiarEstado(int id, [FromBody] CambiarEstadoDto dto)
        {
            try
            {
                if (id != dto.PropuestaId)
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID de la propuesta." });

                await _propuestaService.CambiarEstadoAsync(dto);
                return Ok(new { message = "Estado actualizado correctamente." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Propuesta no encontrada: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Error de validación: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al cambiar estado: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("{id}/estudiantes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<object>> AsignarEstudiante(int id, [FromBody] AsignarEstudianteDto dto)
        {
            try
            {
                if (id != dto.PropuestaId)
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID de la propuesta." });

                await _propuestaService.AsignarEstudianteAsync(dto);
                return Ok(new { message = "Estudiante asignado correctamente." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Recurso no encontrado: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Error de validación: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Operación inválida: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al asignar estudiante: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<object>> EliminarPropuesta(int id)
        {
            try
            {
                var eliminada = await _propuestaService.EliminarPropuestaAsync(id);
                if (!eliminada)
                    return NotFound(new { message = $"No existe propuesta con ID {id}." });

                return Ok(new { message = "Propuesta eliminada correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar propuesta: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
