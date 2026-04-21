namespace TesisTIC.Domain.Entities
{
    public class Modulo
    {
        public int Id { get; set; }
        public int PropuestaId { get; set; }
        public int NumeroModulo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Productos { get; set; }
        public string EstudianteAsignado { get; set; }
        public DateTime? FechaModificacion { get; set; }

        public virtual Propuesta Propuesta { get; set; }
        public virtual ICollection<Actividad> Actividades { get; set; } = new List<Actividad>();
        public virtual ICollection<ObservacionModulo> Observaciones { get; set; } = new List<ObservacionModulo>();
    }
}
