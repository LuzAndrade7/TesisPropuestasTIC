namespace TesisTIC.Domain.Entities
{
    public class Propuesta
    {
        public int Id { get; set; }
        public string NombreProyecto { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Objetivo { get; set; } = string.Empty;
        public string Alcance { get; set; } = string.Empty;
        public int NumeroParticipantes { get; set; }
        public string UnidadAcademica { get; set; } = string.Empty;
        public string Carrera { get; set; } = string.Empty;

        public int ProfesorId { get; set; }
        public virtual Docente Profesor { get; set; }

        public string Estado { get; set; } = "BORRADOR";

        public int? LineaInvestigacionId { get; set; }
        public virtual LineaInvestigacion LineaInvestigacion { get; set; }

        public int? DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEnvioRevision { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public DateTime? FechaRechazo { get; set; }

        // Relaciones
        public virtual ICollection<Componente> Componentes { get; set; } = new List<Componente>();
        public virtual ICollection<PropuestaEstudiante> PropuestasEstudiantes { get; set; } = new List<PropuestaEstudiante>();
        public virtual ICollection<PropuestaAsignatura> PropuestasAsignaturas { get; set; } = new List<PropuestaAsignatura>();
        public virtual ICollection<ObservacionesCpgic> Observaciones { get; set; } = new List<ObservacionesCpgic>();
        public virtual AprobacionesCpgic AprobacionCpgic { get; set; }
    }
}

