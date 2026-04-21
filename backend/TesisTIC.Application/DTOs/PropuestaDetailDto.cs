namespace TesisTIC.Application.DTOs
{
    public class PropuestaDetailDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Proyecto { get; set; }
        public string Descripcion { get; set; }
        public string Objetivo { get; set; }
        public string Alcance { get; set; }
        public int NumeroParticipantes { get; set; }
        public string Departamento { get; set; }
        public string Facultad { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string Estado { get; set; }
        public List<string> Asignaturas { get; set; } = new();
        public List<ModuloDto> Modulos { get; set; } = new();
        public ObservacionPropuestaDto ObservacionDescripcion { get; set; }
        public RechazoPropuestaDto Rechazo { get; set; }
    }
}
