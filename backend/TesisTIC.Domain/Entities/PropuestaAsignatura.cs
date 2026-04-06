namespace TesisTIC.Domain.Entities
{
    public class PropuestaAsignatura
    {
        public int Id { get; set; }

        public int PropuestaId { get; set; }
        public Propuesta Propuesta { get; set; }

        public int AsignaturaId { get; set; }
        public Asignatura Asignatura { get; set; }

        public DateTime FechaAsignacion { get; set; }
    }
}
