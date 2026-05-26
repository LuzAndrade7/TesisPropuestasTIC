using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TesisTIC.Domain.Entities
{
    /// <summary>
    /// HU07 T20: Entidad para relación muchos a muchos entre Propuestas y Estudiantes
    /// Registra qué estudiantes están asignados a cada propuesta
    /// </summary>
    [Table("propuesta_estudiantes")]
    public class PropuestaEstudiante
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("propuesta_id")]
        [Required]
        public int PropuestaId { get; set; }

        [Column("estudiante_id")]
        [Required]
        public int EstudianteId { get; set; }

        [Column("fecha_asignacion")]
        public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

        [Column("asignado_por")]
        [StringLength(200)]
        public string? AsignadoPor { get; set; }

        [Column("estado")]
        [StringLength(50)]
        public string Estado { get; set; } = "ACTIVO"; // ACTIVO, COMPLETADO, RETIRADO

        // Foreign Keys
        [ForeignKey("PropuestaId")]
        public virtual Propuesta? Propuesta { get; set; }

        [ForeignKey("EstudianteId")]
        public virtual Estudiante? Estudiante { get; set; }
    }
}
