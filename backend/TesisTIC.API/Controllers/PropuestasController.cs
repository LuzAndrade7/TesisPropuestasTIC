using Microsoft.AspNetCore.Mvc;
using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;

namespace TesisTIC.API.Controllers;

/// <summary>
/// Controller para operaciones REST de Propuestas
/// Endpoints para CRUD, filtrado y cambio de estados
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PropuestasController : ControllerBase
{
    private readonly IPropuestaService _service;
    private readonly IPropuestaDetalleService _detalleService;
    private readonly ILogger<PropuestasController> _logger;

    public PropuestasController(
        IPropuestaService service,
        IPropuestaDetalleService detalleService,
        ILogger<PropuestasController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _detalleService = detalleService ?? throw new ArgumentNullException(nameof(detalleService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// T04: Obtiene propuestas con filtros opcionales (HU02 - Tablero)
    /// Parámetros:
    /// - estado (opcional): BORRADOR, PENDIENTE, OBSERVADA, APROBADA, RECHAZADA
    /// - ordenar: por defecto por FechaActualizacion descendente
    /// </summary>
    /// <param name="estado">Estado para filtrar (opcional)</param>
    /// <returns>Lista de propuestas resumidas ordenadas por actualización</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<PropuestaResumenDto>>> GetAll([FromQuery] string estado = null)
    {
        try
        {
            // T04: Si se proporciona filtro por estado, validar
            if (!string.IsNullOrWhiteSpace(estado))
            {
                var estadosValidos = new[] { "BORRADOR", "PENDIENTE", "OBSERVADA", "APROBADA", "RECHAZADA" };
                if (!estadosValidos.Contains(estado.ToUpper()))
                {
                    return BadRequest(new { message = "Estado inválido. Estados válidos: BORRADOR, PENDIENTE, OBSERVADA, APROBADA, RECHAZADA" });
                }
            }

            // T04: Obtener todas las propuestas del servicio
            var propuestas = await _service.GetAllAsync();

            // T04: Filtrar por estado si se proporciona
            if (!string.IsNullOrWhiteSpace(estado))
            {
                propuestas = propuestas.Where(p => p.Estado == estado.ToUpper()).ToList();
            }

            // T04: Ordenar por fecha de actualización descendente (más recientes primero)
            var propuestasOrdenadas = propuestas
                .OrderByDescending(p => p.FechaActualizacion)
                .ToList();

            // T04: Mapear a DTO resumen para el tablero (sin información detallada)
            var resumen = propuestasOrdenadas.Select(p => new PropuestaResumenDto
            {
                Id = p.Id,
                NombreProyecto = p.NombreProyecto,
                Estado = p.Estado,
                NumeroParticipantes = p.NumeroParticipantes,
                FechaActualizacion = p.FechaActualizacion,
                FechaCreacion = p.FechaCreacion,
                FechaEnvioRevision = p.FechaEnvioRevision,
                Profesor = p.Profesor != null ? new DocenteResumenDto
                {
                    Id = p.Profesor.Id,
                    NombreCompleto = $"{p.Profesor.Nombres} {p.Profesor.Apellidos}".Trim(),
                    Correo = p.Profesor.Correo
                } : null,
                Asignaturas = p.Asignaturas ?? new List<AsignaturaDto>()
            }).ToList();

            return Ok(resumen);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener propuestas");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene una propuesta por ID con todas sus relaciones
    /// </summary>
    /// <param name="id">ID de la propuesta</param>
    /// <returns>Propuesta encontrada</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropuestaDto>> GetById(int id)
    {
        try
        {
            var propuesta = await _service.GetByIdAsync(id);
            if (propuesta == null)
                return NotFound(new { message = $"Propuesta con ID {id} no encontrada" });

            return Ok(propuesta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener propuesta {propuestaId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene el detalle completo de una propuesta para la vista de consulta.
    /// </summary>
    /// <param name="id">ID de la propuesta</param>
    /// <returns>Detalle completo de la propuesta</returns>
    [HttpGet("{id}/detalle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropuestaDetalleDto>> GetDetalle(int id)
    {
        try
        {
            var detalle = await _detalleService.ObtenerDetalleCompletoAsync(id);
            return Ok(detalle);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalle de propuesta {propuestaId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene propuestas por estado
    /// Estados válidos: BORRADOR, PENDIENTE, OBSERVADA, APROBADA, RECHAZADA
    /// </summary>
    /// <param name="estado">Estado de la propuesta</param>
    /// <returns>Lista de propuestas con ese estado</returns>
    [HttpGet("por-estado/{estado}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<PropuestaDto>>> GetPorEstado(string estado)
    {
        try
        {
            var propuestas = await _service.GetPorEstadoAsync(estado);
            return Ok(propuestas);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener propuestas por estado: {estado}", estado);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene todas las propuestas de un profesor
    /// </summary>
    /// <param name="profesorId">ID del profesor</param>
    /// <returns>Lista de propuestas del profesor</returns>
    [HttpGet("profesor/{profesorId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<PropuestaDto>>> GetPorProfesor(int profesorId)
    {
        try
        {
            var propuestas = await _service.GetPorProfesorAsync(profesorId);
            return Ok(propuestas);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener propuestas del profesor {profesorId}", profesorId);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crea una nueva propuesta
    /// </summary>
    /// <param name="dto">Datos de la nueva propuesta</param>
    /// <returns>Propuesta creada</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PropuestaDto>> Create([FromBody] CreatePropuestaDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var propuesta = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = propuesta.Id }, propuesta);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validación fallida al crear propuesta");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear propuesta: {Message}", ex.Message);
            // Devolver más detalles del error
            var errorMessage = ex.InnerException?.Message ?? ex.Message;
            return StatusCode(500, new
            {
                message = "Error interno del servidor",
                detail = errorMessage,
                type = ex.GetType().Name
            });
        }
    }

    /// <summary>
    /// <summary>
    /// HU05 T14: Actualiza una propuesta existente
    /// Solo permite editar si estado es BORRADOR u OBSERVADA
    /// Realizar actualización parcial: solo campos no null se actualizan
    /// </summary>
    /// <param name="id">ID de la propuesta a actualizar</param>
    /// <param name="dto">Datos a actualizar</param>
    /// <returns>Propuesta actualizada</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PropuestaDto>> Update(int id, [FromBody] UpdatePropuestaDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var propuesta = await _service.UpdateAsync(id, dto);
            _logger.LogInformation("✅ Propuesta {propuestaId} actualizada exitosamente", id);
            return Ok(propuesta);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Validación fallida: {mensaje}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("⚠️ Operación inválida: {mensaje}", ex.Message);
            return StatusCode(403, new { message = ex.Message }); // 403 Forbidden si no se puede editar
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar propuesta {propuestaId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// HU08 T26: Elimina una propuesta TIC (solo si está BORRADOR)
    /// 
    /// Restricciones:
    /// - Solo permite eliminar propuestas en estado BORRADOR
    /// - No permite eliminar: PENDIENTE, OBSERVADA, APROBADA, RECHAZADA
    /// 
    /// Integridad:
    /// - Las cascadas en FK eliminarán automáticamente:
    ///   * propuesta_asignaturas
    ///   * propuesta_estudiantes
    ///   * observaciones_cpgic
    ///   * historial_estados
    /// </summary>
    /// <param name="id">ID de la propuesta a eliminar</param>
    /// <returns>Confirmación de eliminación</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var resultado = await _service.DeleteAsync(id);
            if (!resultado)
                return NotFound(new { message = $"Propuesta con ID {id} no encontrada" });

            _logger.LogInformation("✅ HU08 T26: Propuesta {propuestaId} eliminada correctamente", id);
            return Ok(new
            {
                message = "Propuesta eliminada exitosamente",
                propuestaId = id
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar propuesta {propuestaId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Cambia el estado de una propuesta
    /// </summary>
    /// <param name="id">ID de la propuesta</param>
    /// <param name="dto">Nuevo estado</param>
    /// <returns>Propuesta con estado actualizado</returns>
    [HttpPatch("{id}/estado")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PropuestaDto>> CambiarEstado(int id, [FromBody] UpdateEstadoPropuestaDto dto)
    {
        try
        {
            var propuesta = await _service.CambiarEstadoAsync(id, dto);
            return Ok(propuesta);
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
            _logger.LogError(ex, "Error al cambiar estado de propuesta {propuestaId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// HU03 T07: Envía una propuesta a revisión
    /// Valida que todos los campos requeridos estén completos antes de cambiar estado BORRADOR → PENDIENTE
    /// Validaciones:
    /// - Nombre proyecto: 10-250 caracteres
    /// - Número participantes: 1-5
    /// - Descripción: mínimo 20 caracteres
    /// - Objetivo: no vacío
    /// - Alcance: no vacío
    /// - Mínimo 1 asignatura asignada
    /// </summary>
    /// <param name="id">ID de la propuesta</param>
    /// <param name="dto">DTO con validaciones (puede estar vacío)</param>
    /// <returns>Propuesta actualizada con estado PENDIENTE</returns>
    [HttpPost("{id}/enviar-revision")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PropuestaDto>> EnviarARevision(int id, [FromBody] EnviarARevisionDto dto)
    {
        try
        {
            var propuesta = await _service.EnviarARevisionAsync(id, dto);
            return Ok(propuesta);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validación fallida al enviar propuesta {propuestaId} a revisión", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error operacional al enviar propuesta {propuestaId} a revisión", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar propuesta {propuestaId} a revisión: {Message}", id, ex.Message);
            var errorMessage = ex.InnerException?.Message ?? ex.Message;
            return StatusCode(500, new
            {
                message = "Error interno del servidor",
                detail = errorMessage,
                type = ex.GetType().Name
            });
        }
    }

    /// <summary>
    /// HU04 T12: Reenvía una propuesta después de correcciones
    /// Limpia TODAS las observaciones y cambia estado OBSERVADA → PENDIENTE
    /// Solo se puede reenviar si está en estado OBSERVADA
    /// </summary>
    /// <param name="id">ID de la propuesta</param>
    /// <returns>Propuesta actualizada</returns>
    [HttpPost("{id}/reenviar-despues-observaciones")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PropuestaDto>> ReenviarDespuesDeObservaciones(int id)
    {
        try
        {
            var propuesta = await _service.ReenviarDespuesDeObservacionesAsync(id);
            _logger.LogInformation("✅ Propuesta {propuestaId} reenviada después de observaciones", id);
            return Ok(propuesta);
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
            _logger.LogError(ex, "Error al reenviar propuesta {propuestaId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// HU07 T23-T25: Solicita una nueva aprobación para una propuesta aprobada
    /// Cambia estado APROBADA → PENDIENTE
    /// Se usa cuando hay cambios en estudiantes asignados
    /// </summary>
    /// <param name="id">ID de la propuesta</param>
    /// <param name="dto">Request con motivo de la solicitud</param>
    /// <returns>Propuesta actualizada</returns>
    [HttpPost("{id}/solicitar-nueva-aprobacion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PropuestaDto>> SolicitarNuevaAprobacion(int id, [FromBody] SolicitarNuevaAprobacionDto dto)
    {
        try
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Motivo))
                return BadRequest(new { message = "Debe proporcionar un motivo para solicitar nueva aprobación" });

            var propuesta = await _service.SolicitarNuevaAprobacionAsync(id, dto);
            _logger.LogInformation("✅ HU07 T23: Solicitud nueva aprobación propuesta {propuestaId}. Motivo: {motivo}", id, dto.Motivo);
            return Ok(propuesta);
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
            _logger.LogError(ex, "Error al solicitar nueva aprobación para propuesta {propuestaId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Asigna una asignatura a una propuesta
    /// </summary>
    /// <param name="propuestaId">ID de la propuesta</param>
    /// <param name="asignaturaId">ID de la asignatura</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("{propuestaId}/asignaturas/{asignaturaId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AsignarAsignatura(int propuestaId, int asignaturaId)
    {
        try
        {
            var resultado = await _service.AsignarAsignaturaAsync(propuestaId, asignaturaId);
            if (!resultado)
                return BadRequest(new { message = "La asignatura ya está asignada a esta propuesta" });

            return Ok(new { message = "Asignatura asignada exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar asignatura {asignaturaId} a propuesta {propuestaId}",
                asignaturaId, propuestaId);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Remueve una asignatura de una propuesta
    /// </summary>
    /// <param name="propuestaId">ID de la propuesta</param>
    /// <param name="asignaturaId">ID de la asignatura</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("{propuestaId}/asignaturas/{asignaturaId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RemoverAsignatura(int propuestaId, int asignaturaId)
    {
        try
        {
            var resultado = await _service.RemoverAsignaturaAsync(propuestaId, asignaturaId);
            if (!resultado)
                return BadRequest(new { message = "La asignatura no está asignada a esta propuesta" });

            return Ok(new { message = "Asignatura removida exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al remover asignatura {asignaturaId} de propuesta {propuestaId}",
                asignaturaId, propuestaId);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
