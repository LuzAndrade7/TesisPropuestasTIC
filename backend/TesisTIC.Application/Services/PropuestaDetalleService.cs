using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Services
{
    /// <summary>
    /// HU06 T17: Implementación del servicio de detalle completo de propuesta
    /// Extiende funcionalidad con histórico de estados y datos agrupados
    /// </summary>
    public class PropuestaDetalleService : IPropuestaDetalleService
    {
        private readonly IPropuestaRepository _propuestaRepository;
        private readonly IHistorialEstadoRepository _historialRepository;
        private readonly IObservacionesCpgicRepository _observacionesRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly IAsignaturaRepository _asignaturaRepository;
        private readonly IMapper _mapper;

        public PropuestaDetalleService(
            IPropuestaRepository propuestaRepository,
            IHistorialEstadoRepository historialRepository,
            IObservacionesCpgicRepository observacionesRepository,
            IDocenteRepository docenteRepository,
            IAsignaturaRepository asignaturaRepository,
            IMapper mapper)
        {
            _propuestaRepository = propuestaRepository ?? throw new ArgumentNullException(nameof(propuestaRepository));
            _historialRepository = historialRepository ?? throw new ArgumentNullException(nameof(historialRepository));
            _observacionesRepository = observacionesRepository ?? throw new ArgumentNullException(nameof(observacionesRepository));
            _docenteRepository = docenteRepository ?? throw new ArgumentNullException(nameof(docenteRepository));
            _asignaturaRepository = asignaturaRepository ?? throw new ArgumentNullException(nameof(asignaturaRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// HU06 T17: Obtiene detalle completo de propuesta incluyendo:
        /// - Datos básicos
        /// - Profesor participante
        /// - Asignaturas asociadas
        /// - Observaciones CPGIC
        /// - Histórico de cambios de estado
        /// - Metadata para UI (permisos de acciones)
        /// </summary>
        public async Task<PropuestaDetalleDto> ObtenerDetalleCompletoAsync(int propuestaId)
        {
            if (propuestaId <= 0)
                throw new ArgumentException("ID de propuesta inválido", nameof(propuestaId));

            // 1. Obtener propuesta
            var propuesta = await _propuestaRepository.GetPropuestaFullAsync(propuestaId);
            if (propuesta == null)
                throw new KeyNotFoundException($"Propuesta con ID {propuestaId} no encontrada");

            // 2. Mapear datos basicos
            var detalle = new PropuestaDetalleDto
            {
                Id = propuesta.Id,
                NombreProyecto = propuesta.NombreProyecto,
                Descripcion = propuesta.Descripcion,
                Objetivo = propuesta.Objetivo,
                Alcance = propuesta.Alcance,
                Estado = propuesta.Estado,
                FechaCreacion = propuesta.FechaCreacion,
                FechaActualizacion = propuesta.FechaActualizacion,
                FechaEnvioRevision = propuesta.FechaEnvioRevision,
                NumeroParticipantes = propuesta.NumeroParticipantes
            };

            // 3. Mapear profesor (participante)
            if (propuesta.ProfesorId > 0)
            {
                var profesor = await _docenteRepository.GetByIdAsync(propuesta.ProfesorId);
                if (profesor != null)
                {
                    detalle.Profesor = new ParticipanteDto
                    {
                        Id = profesor.Id,
                        Nombre = $"{profesor.Nombres} {profesor.Apellidos}",
                        Correo = profesor.Correo ?? string.Empty,
                        TituloAcademico = profesor.TituloAcademico,
                        Rol = "Profesor Proponente"
                    };
                }
            }

            // 4. Mapear asignaturas
            if (propuesta.PropuestasAsignaturas != null)
            {
                detalle.Asignaturas = propuesta.PropuestasAsignaturas
                    .Select(pa => new AsignaturaDetalleDto
                    {
                        Id = pa.Asignatura.Id,
                        Codigo = pa.Asignatura.Codigo,
                        Nombre = pa.Asignatura.Nombre,
                        Descripcion = pa.Asignatura.Nombre  // Usar Nombre como descripción
                    })
                    .ToList();
            }

            // 4.1. Mapear componentes o modulos del proyecto
            if (propuesta.Componentes != null && propuesta.Componentes.Any())
            {
                detalle.Componentes = propuesta.Componentes
                    .OrderBy(c => c.Orden)
                    .Select(c => new ComponenteDto
                    {
                        Id = c.Id,
                        PropuestaId = c.PropuestaId,
                        EstudianteId = c.EstudianteId,
                        Nombre = c.Nombre,
                        Descripcion = c.Descripcion,
                        Orden = c.Orden,
                        Estudiante = c.Estudiante == null ? null : new EstudianteDto
                        {
                            Id = c.Estudiante.Id,
                            NombresEstudiante = c.Estudiante.NombresEstudiante,
                            FechaCreacion = c.Estudiante.FechaCreacion
                        },
                        Actividades = c.Actividades
                            .OrderBy(a => a.Numero)
                            .Select(a => new ActividadDto
                            {
                                Id = a.Id,
                                ComponenteId = a.ComponenteId,
                                Numero = a.Numero,
                                Descripcion = a.Descripcion,
                                Horas = a.Horas
                            })
                            .ToList(),
                        ProductosEsperados = c.ProductosEsperados
                            .Select(pe => new ProductoEsperadoDto
                            {
                                Id = pe.Id,
                                ComponenteId = pe.ComponenteId,
                                Descripcion = pe.Descripcion
                            })
                            .ToList()
                    })
                    .ToList();
            }

            // 4.2. Mapear informacion de aprobacion cuando exista
            if (propuesta.AprobacionCpgic != null)
            {
                detalle.ResolucionCpgic = propuesta.AprobacionCpgic.Resolucion;
                detalle.MiembroCpgic = propuesta.AprobacionCpgic.PresidenteCpgic;
                detalle.FechaAprobacion = propuesta.AprobacionCpgic.FechaAprobacion;
                detalle.EstadoAprobacion = propuesta.AprobacionCpgic.EstadoAprobacion;
            }

            // 5. Obtener observaciones
            var observaciones = await _observacionesRepository.GetByPropuestaIdAsync(propuestaId);
            if (observaciones != null && observaciones.Any())
            {
                detalle.Observaciones = observaciones
                    .Select(o => new ObservacionesCpgicResumidaDto
                    {
                        Id = o.Id,
                        Observacion = o.Observacion,
                        RealizadoPor = o.RealizadoPor,
                        FechaObservacion = o.FechaObservacion
                    })
                    .OrderByDescending(o => o.FechaObservacion)
                    .ToList();

                detalle.TotalObservaciones = detalle.Observaciones.Count;
                detalle.UltimaObservacion = detalle.Observaciones.FirstOrDefault()?.FechaObservacion;
            }

            // 6. Obtener historico de estados si la tabla existe en la base actual
            try
            {
                var historial = await _historialRepository.GetByPropuestaIdAsync(propuestaId);
                if (historial != null && historial.Any())
                {
                    detalle.Historial = historial
                        .Select(h => new HistorialEstadoDto
                        {
                            Id = h.Id,
                            EstadoAnterior = h.EstadoAnterior,
                            EstadoNuevo = h.EstadoNuevo,
                            Motivo = h.Motivo,
                            RealizadoPor = h.RealizadoPor,
                            FechaCambio = h.FechaCambio
                        })
                        .ToList();

                    detalle.TotalCambiosEstado = detalle.Historial.Count;
                }
            }
            catch
            {
                detalle.Historial = new List<HistorialEstadoDto>();
                detalle.TotalCambiosEstado = 0;
            }

            // 7. Calcular permisos de acciones según estado
            CalcularPermisosAcciones(detalle, propuesta.Estado);

            return detalle;
        }

        /// <summary>
        /// Calcula qué acciones son permitidas según el estado actual
        /// </summary>
        private void CalcularPermisosAcciones(PropuestaDetalleDto detalle, string estado)
        {
            detalle.EstadosPermitidos = new List<string>();

            switch (estado)
            {
                case "BORRADOR":
                    detalle.PuedeEditar = true;
                    detalle.EstadosPermitidos.Add("Enviar a Revisión");
                    detalle.EstadosPermitidos.Add("Eliminar");
                    break;

                case "PENDIENTE":
                    detalle.PuedeEditar = false;
                    detalle.EstadosPermitidos.Add("Cancelar Revisión");
                    break;

                case "OBSERVADA":
                    detalle.PuedeEditar = true;
                    detalle.PuedeReenviar = true;
                    detalle.EstadosPermitidos.Add("Reenviar con Correcciones");
                    break;

                case "APROBADA":
                    detalle.PuedeEditar = false;
                    detalle.EstadosPermitidos.Add("Archivada");
                    break;

                case "RECHAZADA":
                    detalle.PuedeEditar = false;
                    detalle.EstadosPermitidos.Add("Crear Nueva Propuesta");
                    break;

                default:
                    detalle.PuedeEditar = false;
                    break;
            }

            detalle.PuedeEliminar = estado == "BORRADOR";
        }
    }
}
