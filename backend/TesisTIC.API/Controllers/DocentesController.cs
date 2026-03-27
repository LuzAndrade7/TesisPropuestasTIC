using Microsoft.AspNetCore.Mvc;
using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;

namespace TesisTIC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocentesController : ControllerBase
    {
        private readonly IDocenteRepository _docenteRepository;
        private readonly ILogger<DocentesController> _logger;

        public DocentesController(IDocenteRepository docenteRepository, ILogger<DocentesController> logger)
        {
            _docenteRepository = docenteRepository;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DocenteDto>>> ObtenerTodos()
        {
            try
            {
                var docentes = await _docenteRepository.ObtenerTodosAsync();
                var docentesDto = docentes.Select(d => new DocenteDto
                {
                    Id = d.Id,
                    Nombre = d.Nombre,
                    Apellido = d.Apellido,
                    CorreoInstitucional = d.CorreoInstitucional,
                    Departamento = d.Departamento,
                    Activo = d.Activo
                });
                return Ok(docentesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener docentes: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DocenteDto>> ObtenerPorId(int id)
        {
            try
            {
                var docente = await _docenteRepository.ObtenerPorIdAsync(id);
                if (docente == null)
                    return NotFound(new { message = $"No existe docente con ID {id}." });

                var docenteDto = new DocenteDto
                {
                    Id = docente.Id,
                    Nombre = docente.Nombre,
                    Apellido = docente.Apellido,
                    CorreoInstitucional = docente.CorreoInstitucional,
                    Departamento = docente.Departamento,
                    Activo = docente.Activo
                };
                return Ok(docenteDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener docente: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
