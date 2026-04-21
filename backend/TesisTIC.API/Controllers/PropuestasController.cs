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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListaPropuestasDto>>> ObtenerTodas()
        {
            try
            {
                var propuestas = await _propuestaService.ObtenerTodasAsync();
                return Ok(propuestas);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo propuestas: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PropuestaDetailDto>> ObtenerPropuesta(int id)
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
                _logger.LogError($"Error obteniendo propuesta: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("estadisticas/{docenteId}")]
        public async Task<ActionResult<EstadisticasDto>> ObtenerEstadisticas(int docenteId)
        {
            try
            {
                var stats = await _propuestaService.ObtenerEstadisticasAsync(docenteId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo estadísticas: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<ListaPropuestasDto>>> ObtenerPorEstado(string estado)
        {
            try
            {
                var propuestas = await _propuestaService.ObtenerPorEstadoAsync(estado);
                return Ok(propuestas);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo propuestas por estado: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<PropuestaDetailDto>> CrearPropuesta([FromBody] GuardarPropuestaDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Usar ID de docente 1 como predeterminado (en producción, obtenerlo del usuario autenticado)
                var propuesta = await _propuestaService.CrearPropuestaAsync(dto, docenteId: 1);
                return CreatedAtAction(nameof(ObtenerPropuesta), new { id = propuesta.Id }, propuesta);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Error de validación: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creando propuesta: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PropuestaDetailDto>> ActualizarPropuesta(int id, [FromBody] GuardarPropuestaDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var propuesta = await _propuestaService.ActualizarPropuestaAsync(id, dto);
                return Ok(propuesta);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Propuesta no encontrada: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error actualizando propuesta: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPropuesta(int id)
        {
            try
            {
                var resultado = await _propuestaService.EliminarPropuestaAsync(id);
                if (!resultado)
                    return NotFound(new { message = "Propuesta no encontrada" });

                return Ok(new { message = "Propuesta eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error eliminando propuesta: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("{id}/asignar-estudiantes")]
        public async Task<ActionResult<PropuestaDetailDto>> AsignarEstudiantes(int id, [FromBody] AsignarEstudiantesDto dto)
        {
            try
            {
                var propuesta = await _propuestaService.AsignarEstudiantesAsync(id, dto);
                return Ok(propuesta);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error asignando estudiantes: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPatch("{id}/cambiar-estado")]
        public async Task<ActionResult<PropuestaDetailDto>> CambiarEstado(int id, [FromBody] dynamic data)
        {
            try
            {
                string estado = data.estado;
                var propuesta = await _propuestaService.CambiarEstadoAsync(id, estado);
                return Ok(propuesta);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error cambiando estado: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
