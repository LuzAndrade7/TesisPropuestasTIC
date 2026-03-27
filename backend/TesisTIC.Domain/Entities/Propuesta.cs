namespace TesisTIC.Domain.Entities
{
    public class Propuesta
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Objetivo { get; set; }
        public string Alcance { get; set; }
        public string ComponentesActividadesProductos { get; set; }
        
        public int DocenteId { get; set; }
        public Docente Docente { get; set; }
        
        public int EstadoId { get; set; }
        public Estado Estado { get; set; }
        
        public int? LineaInvestigacionId { get; set; }
        public LineaInvestigacion LineaInvestigacion { get; set; }
        
        public string? Observaciones { get; set; }
        public int NumeroParticipantes { get; set; }
        public string Departamento { get; set; }
        public string Facultad { get; set; }
        
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEnvioPrimera { get; set; }

        public ICollection<PropuestaEstudiante> EstudiantesAsignados { get; set; } = new List<PropuestaEstudiante>();
        public ICollection<Asignatura> Asignaturas { get; set; } = new List<Asignatura>();
    }
}
