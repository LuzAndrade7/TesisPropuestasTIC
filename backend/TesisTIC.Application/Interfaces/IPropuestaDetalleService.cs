using TesisTIC.Application.DTOs;

namespace TesisTIC.Application.Interfaces
{
    /// <summary>
    /// HU06 T17: Interfaz para servicio de detalle completo de propuesta
    /// </summary>
    public interface IPropuestaDetalleService
    {
        Task<PropuestaDetalleDto> ObtenerDetalleCompletoAsync(int propuestaId);
    }
}
