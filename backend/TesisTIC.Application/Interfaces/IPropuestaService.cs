using TesisTIC.Application.DTOs;

namespace TesisTIC.Application.Interfaces
{
    public interface IPropuestaService
    {
        Task<PropuestaDto> CrearPropuestaAsync(CrearPropuestaDto dto);
        Task<PropuestaDto> ObtenerPropuestaAsync(int id);
        Task<IEnumerable<PropuestaDto>> ObtenerTodasAsync();
        Task<IEnumerable<PropuestaDto>> ObtenerPorDocenteAsync(int docenteId);
        Task<IEnumerable<PropuestaDto>> ObtenerPorEstadoAsync(int estadoId);
        Task<PropuestaDto> ActualizarPropuestaAsync(ActualizarPropuestaDto dto);
        Task<bool> CambiarEstadoAsync(CambiarEstadoDto dto);
        Task<bool> AsignarEstudianteAsync(AsignarEstudianteDto dto);
        Task<bool> EliminarPropuestaAsync(int id);
    }
}
