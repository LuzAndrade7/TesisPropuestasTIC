using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;
using TesisTIC.Application.Services;
using TesisTIC.Domain.Entities;
using TesisTIC.Infrastructure.Data;

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
    private readonly IEstudianteRepository _estudianteRepository;
    private readonly TesisTicDbContext _context;
    private readonly IPropuestaEstudianteService _propuestaEstudianteService; // HU07 T20
    private readonly ILogger<EstudiantesController> _logger;

    public EstudiantesController(
        IEstudianteService service,
        IEstudianteRepository estudianteRepository,
        TesisTicDbContext context,
        IPropuestaEstudianteService propuestaEstudianteService, // HU07 T20
        ILogger<EstudiantesController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _estudianteRepository = estudianteRepository ?? throw new ArgumentNullException(nameof(estudianteRepository));
        _context = context ?? throw new ArgumentNullException(nameof(context));
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
            var estudiantes = await _context.Componentes
                .Include(c => c.Estudiante)
                .Where(c => c.PropuestaId == propuestaId && c.EstudianteId.HasValue && c.Estudiante != null)
                .OrderBy(c => c.Orden)
                .Select(c => new PropuestaEstudianteDto
                {
                    Id = c.Id,
                    PropuestaId = propuestaId,
                    Estudiante = new EstudianteDto
                    {
                        Id = c.Estudiante!.Id,
                        NombresEstudiante = c.Estudiante.NombresEstudiante,
                        FechaCreacion = c.Estudiante.FechaCreacion
                    },
                    FechaAsignacion = c.Estudiante.FechaCreacion,
                    AsignadoPor = "Sistema",
                    Estado = "ACTIVO"
                })
                .ToListAsync();

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
            if (request == null)
                return BadRequest(new { message = "Solicitud inválida" });

            if ((request.EstudianteIds == null || request.EstudianteIds.Count == 0) &&
                (request.NombresEstudiante == null || request.NombresEstudiante.Count == 0))
                return BadRequest(new { message = "Debe proporcionar al menos un estudiante" });

            var estudianteIds = request.EstudianteIds ?? new List<int>();

            if (request.NombresEstudiante != null && request.NombresEstudiante.Any())
            {
                foreach (var nombre in request.NombresEstudiante
                    .Where(nombre => !string.IsNullOrWhiteSpace(nombre))
                    .Select(nombre => nombre.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    var existente = await _estudianteRepository.GetByNombreEstudianteAsync(nombre);
                    if (existente == null)
                    {
                        var creado = await _service.CreateAsync(new CreateUpdateEstudianteDto
                        {
                            NombresEstudiante = nombre
                        });
                        estudianteIds.Add(creado.Id);
                    }
                    else
                    {
                        estudianteIds.Add(existente.Id);
                    }
                }
            }

            estudianteIds = estudianteIds.Distinct().ToList();

            if (estudianteIds.Count > 5)
                return BadRequest(new { message = "No se pueden asignar más de 5 estudiantes por propuesta" });

            var propuesta = await _context.Propuestas
                .Include(p => p.Componentes)
                .ThenInclude(c => c.Estudiante)
                .FirstOrDefaultAsync(p => p.Id == propuestaId);

            if (propuesta == null)
                return NotFound(new { message = $"Propuesta con ID {propuestaId} no encontrada" });

            if (propuesta.Estado != "APROBADA" && propuesta.Estado != "PENDIENTE")
                return BadRequest(new { message = $"Solo propuestas APROBADAS o PENDIENTES pueden asignar estudiantes. Estado actual: {propuesta.Estado}" });

            var estadoAnterior = propuesta.Estado;

            var modulos = propuesta.Componentes.OrderBy(c => c.Orden).ToList();
            if (!modulos.Any())
                return BadRequest(new { message = "La propuesta no tiene modulos registrados para asignar estudiantes" });

            if (propuesta.NumeroParticipantes < 2 || propuesta.NumeroParticipantes > 5)
                return BadRequest(new { message = "La propuesta debe tener entre 2 y 5 participantes" });

            if (modulos.Count != propuesta.NumeroParticipantes)
                return BadRequest(new
                {
                    message = $"La propuesta debe tener exactamente {propuesta.NumeroParticipantes} módulos, uno por participante"
                });

            if (estudianteIds.Count != propuesta.NumeroParticipantes)
                return BadRequest(new
                {
                    message = $"Debe asignar exactamente {propuesta.NumeroParticipantes} estudiantes, uno por módulo"
                });

            foreach (var modulo in modulos)
            {
                modulo.EstudianteId = null;
            }

            for (var indice = 0; indice < estudianteIds.Count && indice < modulos.Count; indice++)
            {
                modulos[indice].EstudianteId = estudianteIds[indice];
            }

            if (estadoAnterior == "APROBADA")
            {
                var ahora = FechaEcuador.Ahora();
                var motivo = string.IsNullOrWhiteSpace(request.Motivo)
                    ? "Asignacion o actualizacion de estudiantes en una propuesta ya aprobada."
                    : request.Motivo.Trim();

                const string prefijoObservacionNuevaAprobacion = "Observacion para el miembro de la CPGIC:";

                propuesta.Estado = "PENDIENTE";
                propuesta.FechaActualizacion = ahora;
                propuesta.FechaEnvioRevision = ahora;
                var observacionesPrevias = await _context.Observaciones
                    .Where(o => o.PropuestaId == propuesta.Id &&
                        (o.Observacion.StartsWith(prefijoObservacionNuevaAprobacion) ||
                         o.Observacion.StartsWith("Solicitud de nueva aprobacion:")))
                    .ToListAsync();

                if (observacionesPrevias.Any())
                {
                    _context.Observaciones.RemoveRange(observacionesPrevias);
                }

                _context.Observaciones.Add(new ObservacionesCpgic
                {
                    PropuestaId = propuesta.Id,
                    Observacion = $"{prefijoObservacionNuevaAprobacion} esta propuesta ya fue aprobada previamente. Revisar solo el nuevo cambio registrado en la asignacion o actualizacion de estudiantes. Motivo: {motivo}",
                    RealizadoPor = request.RealizadoPor ?? "Docente",
                    FechaObservacion = ahora
                });
            }

            await _context.SaveChangesAsync();

            var asignaciones = modulos
                .Where(m => m.EstudianteId.HasValue)
                .Select(m => new PropuestaEstudianteDto
                {
                    Id = m.Id,
                    PropuestaId = propuestaId,
                    Estudiante = new EstudianteDto
                    {
                        Id = m.EstudianteId!.Value,
                        NombresEstudiante = m.Estudiante?.NombresEstudiante ?? string.Empty
                    },
                    FechaAsignacion = DateTime.UtcNow,
                    AsignadoPor = request.RealizadoPor ?? "Sistema",
                    Estado = "ACTIVO"
                })
                .ToList();

            _logger.LogInformation("HU07 T20: Asignados {count} estudiantes a modulos de propuesta {propuestaId}",
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
