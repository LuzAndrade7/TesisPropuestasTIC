using System;

namespace TesisTIC.Application.DTOs
{
    /// <summary>
    /// T04: DTO para resumen de propuesta en tablero (HU02)
    /// Contiene solo información esencial, sin módulos/componentes detallados
    /// Ubicación: TesisTIC.Application/DTOs/PropuestaResumenDto.cs
    /// </summary>
    public class PropuestaResumenDto
    {
        /// <summary>
        /// ID único de la propuesta
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre/Título del proyecto
        /// </summary>
        public string NombreProyecto { get; set; }

        /// <summary>
        /// Estado de la propuesta: BORRADOR, PENDIENTE, OBSERVADA, APROBADA, RECHAZADA
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Número de participantes
        /// </summary>
        public int NumeroParticipantes { get; set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime FechaActualizacion { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Fecha de envío a revisión (si aplica)
        /// </summary>
        public DateTime? FechaEnvioRevision { get; set; }

        /// <summary>
        /// Información resumida del profesor proponente
        /// </summary>
        public DocenteResumenDto Profesor { get; set; }

        /// <summary>
        /// Información de asignaturas asociadas
        /// </summary>
        public List<AsignaturaDto> Asignaturas { get; set; } = new List<AsignaturaDto>();
    }

    /// <summary>
    /// T04: DTO resumido para Docente (en tablero)
    /// </summary>
    public class DocenteResumenDto
    {
        /// <summary>
        /// ID del docente
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre completo del docente
        /// </summary>
        public string NombreCompleto { get; set; }

        /// <summary>
        /// Correo del docente
        /// </summary>
        public string Correo { get; set; }
    }
}
