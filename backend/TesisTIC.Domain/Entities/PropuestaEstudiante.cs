namespace TesisTIC.Domain.Entities
{
    public class PropuestaEstudiante
    {
        public int Id { get; set; }
        
        public int PropuestaId { get; set; }
        public Propuesta Propuesta { get; set; }
        
        public int EstudianteId { get; set; }
        public Estudiante Estudiante { get; set; }
        
        public DateTime FechaAsignacion { get; set; }
    }
}
