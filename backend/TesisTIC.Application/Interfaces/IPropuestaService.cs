using TesisTIC.Application.DTOs;

namespace TesisTIC.Application.Interfaces
{
    public interface IPropuestaService
    {
        Task<PropuestaDetailDto> ObtenerPropuestaAsync(int id);
        Task<IEnumerable<ListaPropuestasDto>> ObtenerTodasAsync();
        Task<IEnumerable<ListaPropuestasDto>> ObtenerPorDocenteAsync(int docenteId);
        Task<IEnumerable<ListaPropuestasDto>> ObtenerPorEstadoAsync(string estado);
        Task<EstadisticasDto> ObtenerEstadisticasAsync(int docenteId);
        Task<PropuestaDetailDto> CrearPropuestaAsync(GuardarPropuestaDto dto, int docenteId);
        Task<PropuestaDetailDto> ActualizarPropuestaAsync(int id, GuardarPropuestaDto dto);
        Task<bool> EliminarPropuestaAsync(int id);
        Task<PropuestaDetailDto> AsignarEstudiantesAsync(int id, AsignarEstudiantesDto dto);
        Task<PropuestaDetailDto> CambiarEstadoAsync(int id, string estado);
    }
}
