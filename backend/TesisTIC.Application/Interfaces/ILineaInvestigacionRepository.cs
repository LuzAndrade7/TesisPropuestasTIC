using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Interfaces
{
    public interface ILineaInvestigacionRepository
    {
        Task<LineaInvestigacion> ObtenerPorIdAsync(int id);
        Task<IEnumerable<LineaInvestigacion>> ObtenerTodoAsync();
    }
}
