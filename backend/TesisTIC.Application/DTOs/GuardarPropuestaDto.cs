namespace TesisTIC.Application.DTOs
{
    public class GuardarPropuestaDto
    {
        public int? Id { get; set; }
        public string Titulo { get; set; }
        public string Proyecto { get; set; }
        public string Descripcion { get; set; }
        public string Objetivo { get; set; }
        public string Alcance { get; set; }
        public int NumeroParticipantes { get; set; }
        public List<string> Asignaturas { get; set; } = new();
        public List<ModuloInputDto> Modulos { get; set; } = new();
    }

    public class ModuloInputDto
    {
        public int? Id { get; set; }
        public int NumeroModulo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Productos { get; set; }
        public List<ActividadInputDto> Actividades { get; set; } = new();
    }

    public class ActividadInputDto
    {
        public int? Id { get; set; }
        public string Descripcion { get; set; }
        public int Horas { get; set; }
    }
}
