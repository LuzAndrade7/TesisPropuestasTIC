using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Services
{
    /// <summary>
    /// DEPRECATED: Use PropuestaServiceCompleto instead.
    /// This service is kept for backwards compatibility only.
    /// </summary>
    public class PropuestaService : IPropuestaService
    {
        public Task<PropuestaDetailDto> ObtenerPropuestaAsync(int id)
            => throw new NotImplementedException("Use PropuestaServiceCompleto instead");

        public Task<IEnumerable<ListaPropuestasDto>> ObtenerTodasAsync()
            => throw new NotImplementedException("Use PropuestaServiceCompleto instead");

        public Task<IEnumerable<ListaPropuestasDto>> ObtenerPorDocenteAsync(int docenteId)
            => throw new NotImplementedException("Use PropuestaServiceCompleto instead");

        public Task<IEnumerable<ListaPropuestasDto>> ObtenerPorEstadoAsync(string estado)
            => throw new NotImplementedException("Use PropuestaServiceCompleto instead");

        public Task<EstadisticasDto> ObtenerEstadisticasAsync(int docenteId)
            => throw new NotImplementedException("Use PropuestaServiceCompleto instead");

        public Task<PropuestaDetailDto> CrearPropuestaAsync(GuardarPropuestaDto dto, int docenteId)
            => throw new NotImplementedException("Use PropuestaServiceCompleto instead");

        public Task<PropuestaDetailDto> ActualizarPropuestaAsync(int id, GuardarPropuestaDto dto)
            => throw new NotImplementedException("Use PropuestaServiceCompleto instead");

        public Task<bool> EliminarPropuestaAsync(int id)
            => throw new NotImplementedException("Use PropuestaServiceCompleto instead");

        public Task<PropuestaDetailDto> AsignarEstudiantesAsync(int id, AsignarEstudiantesDto dto)
            => throw new NotImplementedException("Use PropuestaServiceCompleto instead");

        public Task<PropuestaDetailDto> CambiarEstadoAsync(int id, string estado)
            => throw new NotImplementedException("Use PropuestaServiceCompleto instead");
    }
}
