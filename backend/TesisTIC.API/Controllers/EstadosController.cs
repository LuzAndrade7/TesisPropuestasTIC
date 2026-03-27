using Microsoft.AspNetCore.Mvc;
using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;

namespace TesisTIC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstadosController : ControllerBase
    {
        private readonly IEstadoRepository _estadoRepository;
        private readonly ILogger<EstadosController> _logger;

        public EstadosController(IEstadoRepository estadoRepository, ILogger<EstadosController> logger)
        {
            _estadoRepository = estadoRepository;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EstadoDto>>> ObtenerTodos()
        {
            try
            {
                var estados = await _estadoRepository.ObtenerTodosAsync();
                var estadosDto = estados.Select(e => new EstadoDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Descripcion = e.Descripcion
                });
                return Ok(estadosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener estados: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
