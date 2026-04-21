namespace TesisTIC.Domain.Entities
{
    public class Actividad
    {
        public int Id { get; set; }
        public int ModuloId { get; set; }
        public string Descripcion { get; set; }
        public int Horas { get; set; }

        public virtual Modulo Modulo { get; set; }
    }
}
