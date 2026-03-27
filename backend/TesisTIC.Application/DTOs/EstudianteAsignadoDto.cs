namespace TesisTIC.Application.DTOs
{
    public class EstudianteAsignadoDto
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public string NombreEstudiante { get; set; }
        public string ApellidoEstudiante { get; set; }
        public string MatriculaEstudiante { get; set; }
        public DateTime FechaAsignacion { get; set; }
    }
}
