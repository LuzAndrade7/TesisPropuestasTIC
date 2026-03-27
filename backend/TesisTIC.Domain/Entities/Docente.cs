namespace TesisTIC.Domain.Entities
{
    public class Docente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string CorreoInstitucional { get; set; }
        public string CorreoPersonal { get; set; }
        public string Cedula { get; set; }
        public string NumeroEmpleado { get; set; }
        public string Departamento { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaAtualizacion { get; set; }

        public ICollection<Propuesta> Propuestas { get; set; } = new List<Propuesta>();
    }
}
