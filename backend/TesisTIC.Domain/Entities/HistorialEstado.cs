using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TesisTIC.Domain.Entities
{
    /// <summary>
    /// HU06 T17: Entidad para registrar cambios de estado de propuestas
    /// Crea un historial auditado de todas las transiciones de estado
    /// </summary>
    [Table("historial_estados")]
    public class HistorialEstado
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("propuesta_id")]
        [Required]
        public int PropuestaId { get; set; }

        [Column("estado_anterior")]
        [StringLength(50)]
        public string? EstadoAnterior { get; set; }

        [Column("estado_nuevo")]
        [StringLength(50)]
        [Required]
        public string EstadoNuevo { get; set; } = string.Empty;

        [Column("motivo")]
        public string? Motivo { get; set; }

        [Column("realizado_por")]
        [StringLength(200)]
        public string? RealizadoPor { get; set; }

        [Column("fecha_cambio")]
        public DateTime FechaCambio { get; set; } = DateTime.UtcNow;

        // Foreign Key
        [ForeignKey("PropuestaId")]
        public virtual Propuesta? Propuesta { get; set; }
    }
}
