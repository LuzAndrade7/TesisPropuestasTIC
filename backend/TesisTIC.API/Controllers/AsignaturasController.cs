using Microsoft.AspNetCore.Mvc;
using TesisTIC.Application.Interfaces;

namespace TesisTIC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsignaturasController : ControllerBase
    {
        private readonly IAsignaturaRepository _asignaturaRepository;
        private readonly ILogger<AsignaturasController> _logger;

        public AsignaturasController(IAsignaturaRepository asignaturaRepository, ILogger<AsignaturasController> logger)
        {
            _asignaturaRepository = asignaturaRepository;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<object>>> ObtenerTodas()
        {
            try
            {
                var asignaturas = await _asignaturaRepository.ObtenerTodosAsync();
                var asignaturasDto = asignaturas.Select(a => new
                {
                    a.Id,
                    a.Nombre,
                    a.Codigo
                });
                return Ok(asignaturasDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener asignaturas: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
