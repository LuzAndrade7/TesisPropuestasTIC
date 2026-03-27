namespace TesisTIC.Application.DTOs
{
    public class PropuestaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Objetivo { get; set; }
        public string Alcance { get; set; }
        public string ComponentesActividadesProductos { get; set; }
        public int DocenteId { get; set; }
        public string DocenteNombre { get; set; }
        public int EstadoId { get; set; }
        public string EstadoNombre { get; set; }
        public int? LineaInvestigacionId { get; set; }
        public string? LineaInvestigacionNombre { get; set; }
        public string? Observaciones { get; set; }
        public int NumeroParticipantes { get; set; }
        public string Departamento { get; set; }
        public string Facultad { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEnvioPrimera { get; set; }
        public List<EstudianteAsignadoDto> EstudiantesAsignados { get; set; } = new List<EstudianteAsignadoDto>();
        public List<AsignaturaDto> Asignaturas { get; set; } = new List<AsignaturaDto>();
    }
}
