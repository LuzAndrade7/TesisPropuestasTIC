namespace TesisTIC.Domain.Entities
{
    public class Propuesta
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

        public int DocenteId { get; set; }
        public virtual Docente Docente { get; set; }

        public int EstadoId { get; set; }
        public virtual Estado Estado { get; set; }

        public int? LineaInvestigacionId { get; set; }
        public virtual LineaInvestigacion LineaInvestigacion { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEnvioPrimera { get; set; }

        public virtual ICollection<Modulo> Modulos { get; set; } = new List<Modulo>();
        public virtual ICollection<PropuestaEstudiante> EstudiantesAsignados { get; set; } = new List<PropuestaEstudiante>();
        public virtual ICollection<PropuestaAsignatura> AsignaturasAsignadas { get; set; } = new List<PropuestaAsignatura>();
        public virtual ICollection<ObservacionPropuesta> Observaciones { get; set; } = new List<ObservacionPropuesta>();
        public virtual RechazoPropuesta Rechazo { get; set; }
    }
}

