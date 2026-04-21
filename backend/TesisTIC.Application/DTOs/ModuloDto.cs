namespace TesisTIC.Application.DTOs
{
    public class ModuloDto
    {
        public int Id { get; set; }
        public int NumeroModulo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Productos { get; set; }
        public string EstudianteAsignado { get; set; }
        public List<ActividadDto> Actividades { get; set; } = new();
        public ObservacionModuloDto Observaciones { get; set; }
    }
}
