using System;

namespace TesisTIC.Application.DTOs
{
    /// <summary>
    /// HU07 T20: DTO para propuesta con estudiantes asignados
    /// </summary>
    public class PropuestaEstudianteDto
    {
        public int Id { get; set; }
        public int PropuestaId { get; set; }
        public EstudianteDto Estudiante { get; set; } = new();
        public DateTime FechaAsignacion { get; set; }
        public string? AsignadoPor { get; set; }
        public string Estado { get; set; } = "ACTIVO";
    }

    /// <summary>
    /// HU07 T20: Request para asignar estudiantes a una propuesta
    /// </summary>
    public class AsignarEstudiantesRequest
    {
        public int PropuestaId { get; set; }
        public List<int> EstudianteIds { get; set; } = new();
        public string? Motivo { get; set; }
        public string? RealizadoPor { get; set; }
    }

    /// <summary>
    /// HU07 T20: Request para actualizar asignación de estudiantes
    /// </summary>
    public class ActualizarEstudiantesRequest
    {
        public int PropuestaId { get; set; }
        public List<int> EstudianteIds { get; set; } = new();
        public string Motivo { get; set; } = string.Empty;
        public string? RealizadoPor { get; set; }
    }

    /// <summary>
    /// HU07 T23: Request para solicitar nueva aprobación después de cambios
    /// </summary>
    public class SolicitarNuevaAprobacionRequest
    {
        public int PropuestaId { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string? RealizadoPor { get; set; }
    }
}
