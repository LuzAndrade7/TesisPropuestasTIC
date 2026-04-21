namespace TesisTIC.Application.DTOs
{
    public class AsignarEstudiantesDto
    {
        public List<AsignacionModuloDto> Modulos { get; set; } = new();
    }

    public class AsignacionModuloDto
    {
        public int ModuloId { get; set; }
        public string EstudianteNombre { get; set; }
    }
}
