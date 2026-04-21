namespace TesisTIC.Domain.Entities
{
    public class ObservacionModulo
    {
        public int Id { get; set; }
        public int ModuloId { get; set; }
        public string Autor { get; set; }
        public string Texto { get; set; }
        public DateTime Fecha { get; set; }

        public virtual Modulo Modulo { get; set; }
    }
}
