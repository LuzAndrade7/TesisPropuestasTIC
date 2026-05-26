using System;
using System.Collections.Generic;

namespace TesisTIC.Application.DTOs
{
    /// <summary>
    /// HU06 T17: DTO para historial de cambios de estado
    /// </summary>
    public class HistorialEstadoDto
    {
        public int Id { get; set; }
        public string? EstadoAnterior { get; set; }
        public string EstadoNuevo { get; set; } = string.Empty;
        public string? Motivo { get; set; }
        public string? RealizadoPor { get; set; }
        public DateTime FechaCambio { get; set; }
    }

    /// <summary>
    /// HU06 T17: DTO para observación CPGIC (resumida)
    /// </summary>
    public class ObservacionesCpgicResumidaDto
    {
        public int Id { get; set; }
        public string Observacion { get; set; } = string.Empty;
        public string? RealizadoPor { get; set; }
        public DateTime FechaObservacion { get; set; }
    }

    /// <summary>
    /// HU06 T17: DTO para participantes (información del profesor)
    /// </summary>
    public class ParticipanteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string? TituloAcademico { get; set; }
        public string Rol { get; set; } = "Profesor Proponente";
    }

    /// <summary>
    /// HU06 T17: DTO para asignatura (información resumida)
    /// </summary>
    public class AsignaturaDetalleDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    /// <summary>
    /// HU06 T17: DTO completo para detalle de propuesta
    /// Incluye todos los datos relacionados: participantes, asignaturas, observaciones, histórico
    /// </summary>
    public class PropuestaDetalleDto
    {
        // Datos Básicos
        public int Id { get; set; }
        public string NombreProyecto { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Objetivo { get; set; } = string.Empty;
        public string Alcance { get; set; } = string.Empty;

        // Estado y Fechas
        public string Estado { get; set; } = "BORRADOR";
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEnvioRevision { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        // Participantes
        public int NumeroParticipantes { get; set; }
        public ParticipanteDto? Profesor { get; set; }

        // Asignaturas Relacionadas
        public List<AsignaturaDetalleDto> Asignaturas { get; set; } = new();

        // Observaciones CPGIC (si las hay)
        public List<ObservacionesCpgicResumidaDto> Observaciones { get; set; } = new();

        // Histórico de Estados
        public List<HistorialEstadoDto> Historial { get; set; } = new();

        // Metadata Adicional
        public int TotalObservaciones { get; set; }
        public DateTime? UltimaObservacion { get; set; }
        public int TotalCambiosEstado { get; set; }

        // Datos para UI (estados permitidos en este punto)
        public List<string> EstadosPermitidos { get; set; } = new();
        public bool PuedeEditar { get; set; }
        public bool PuedeReenviar { get; set; }
        public bool PuedeEliminar { get; set; }
    }
}
