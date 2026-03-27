namespace TesisTIC.Application.DTOs
{
    public class DocenteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string CorreoInstitucional { get; set; }
        public string Departamento { get; set; }
        public bool Activo { get; set; }
    }
}
