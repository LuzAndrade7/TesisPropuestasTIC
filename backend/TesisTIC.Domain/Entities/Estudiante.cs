namespace TesisTIC.Domain.Entities
{
    public class Estudiante
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Matricula { get; set; }
        public string CorreoInstitucional { get; set; }
        public string Cedula { get; set; }
        public DateTime FechaCreacion { get; set; }

        public ICollection<PropuestaEstudiante> PropuestasAsignadas { get; set; } = new List<PropuestaEstudiante>();
    }
}
