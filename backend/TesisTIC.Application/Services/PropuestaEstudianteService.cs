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
    /// HU07 T20: Implementación del servicio de asignación de estudiantes
    /// </summary>
    public class PropuestaEstudianteService : IPropuestaEstudianteService
    {
        private readonly IPropuestaEstudianteRepository _propuestaEstudianteRepository;
        private readonly IEstudianteRepository _estudianteRepository;
        private readonly IPropuestaRepository _propuestaRepository;
        private readonly IMapper _mapper;

        public PropuestaEstudianteService(
            IPropuestaEstudianteRepository propuestaEstudianteRepository,
            IEstudianteRepository estudianteRepository,
            IPropuestaRepository propuestaRepository,
            IMapper mapper)
        {
            _propuestaEstudianteRepository = propuestaEstudianteRepository ?? throw new ArgumentNullException(nameof(propuestaEstudianteRepository));
            _estudianteRepository = estudianteRepository ?? throw new ArgumentNullException(nameof(estudianteRepository));
            _propuestaRepository = propuestaRepository ?? throw new ArgumentNullException(nameof(propuestaRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // ===== LECTURA =====

        /// <summary>
        /// Obtiene todos los estudiantes asignados a una propuesta
        /// </summary>
        public async Task<List<PropuestaEstudianteDto>> ObtenerEstudiantesPorPropuestaAsync(int propuestaId)
        {
            if (propuestaId <= 0)
                throw new ArgumentException("ID de propuesta inválido", nameof(propuestaId));

            var asignaciones = await _propuestaEstudianteRepository.GetByPropuestaIdAsync(propuestaId);
            return asignaciones
                .Select(a => new PropuestaEstudianteDto
                {
                    Id = a.Id,
                    PropuestaId = a.PropuestaId,
                    Estudiante = _mapper.Map<EstudianteDto>(a.Estudiante),
                    FechaAsignacion = a.FechaAsignacion,
                    AsignadoPor = a.AsignadoPor,
                    Estado = a.Estado
                })
                .ToList();
        }

        /// <summary>
        /// Obtiene todos los estudiantes del sistema
        /// </summary>
        public async Task<List<EstudianteDto>> ObtenerTodosEstudiantesAsync()
        {
            var estudiantes = await _estudianteRepository.GetAllAvailableAsync();
            return _mapper.Map<List<EstudianteDto>>(estudiantes);
        }

        /// <summary>
        /// Busca estudiantes por nombre, apellido o correo
        /// </summary>
        public async Task<List<EstudianteDto>> BuscarEstudiantesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await ObtenerTodosEstudiantesAsync();

            var estudiantes = await _estudianteRepository.SearchAsync(searchTerm);
            return _mapper.Map<List<EstudianteDto>>(estudiantes);
        }

        /// <summary>
        /// Obtiene estudiantes que aún no están asignados a una propuesta
        /// </summary>
        public async Task<List<EstudianteDto>> ObtenerEstudiantesDisponiblesAsync(int propuestaId)
        {
            if (propuestaId <= 0)
                throw new ArgumentException("ID de propuesta inválido", nameof(propuestaId));

            var estudiantes = await _estudianteRepository.GetNoAsignadosAsync(propuestaId);
            return _mapper.Map<List<EstudianteDto>>(estudiantes);
        }

        // ===== ESCRITURA =====

        /// <summary>
        /// Asigna estudiantes a una propuesta (máximo 5)
        /// </summary>
        public async Task<List<PropuestaEstudianteDto>> AsignarEstudiantesAsync(
            int propuestaId,
            List<int> estudianteIds,
            string? motivo = null,
            string? realizadoPor = null)
        {
            if (propuestaId <= 0)
                throw new ArgumentException("ID de propuesta inválido", nameof(propuestaId));

            if (estudianteIds == null || estudianteIds.Count == 0)
                throw new ArgumentException("Debe seleccionar al menos un estudiante", nameof(estudianteIds));

            if (estudianteIds.Count > 5)
                throw new ArgumentException("No se pueden asignar más de 5 estudiantes por propuesta", nameof(estudianteIds));

            // Validar que propuesta existe y está en estado correcto
            var propuesta = await _propuestaRepository.GetByIdAsync(propuestaId);
            if (propuesta == null)
                throw new KeyNotFoundException($"Propuesta con ID {propuestaId} no encontrada");

            // Solo APROBADA o PENDIENTE pueden asignar estudiantes
            if (propuesta.Estado != "APROBADA" && propuesta.Estado != "PENDIENTE")
                throw new InvalidOperationException($"Solo propuestas APROBADAS o PENDIENTES pueden asignar estudiantes. Estado actual: {propuesta.Estado}");

            // Limpiar asignaciones anteriores si existen
            await _propuestaEstudianteRepository.DeleteByPropuestaIdAsync(propuestaId);

            // Crear nuevas asignaciones
            var asignaciones = new List<(PropuestaEstudiante Asignacion, Estudiante Estudiante)>();
            foreach (var estudianteId in estudianteIds)
            {
                var estudiante = await _estudianteRepository.GetByIdAsync(estudianteId);
                if (estudiante == null)
                    continue;

                var asignacion = new PropuestaEstudiante
                {
                    PropuestaId = propuestaId,
                    EstudianteId = estudianteId,
                    FechaAsignacion = DateTime.UtcNow,
                    AsignadoPor = realizadoPor ?? "Sistema",
                    Estado = "ACTIVO"
                };

                asignaciones.Add((asignacion, estudiante));
            }

            // Guardar asignaciones
            foreach (var item in asignaciones)
            {
                await _propuestaEstudianteRepository.CreateAsync(item.Asignacion);
            }

            return asignaciones
                .Select(item => new PropuestaEstudianteDto
                {
                    Id = item.Asignacion.Id,
                    PropuestaId = item.Asignacion.PropuestaId,
                    Estudiante = new EstudianteDto
                    {
                        Id = item.Asignacion.EstudianteId,
                        NombresEstudiante = item.Estudiante.NombresEstudiante,
                        FechaCreacion = item.Estudiante.FechaCreacion
                    },
                    FechaAsignacion = item.Asignacion.FechaAsignacion,
                    AsignadoPor = item.Asignacion.AsignadoPor,
                    Estado = item.Asignacion.Estado
                })
                .ToList();
        }

        /// <summary>
        /// Actualiza la asignación de estudiantes (cuando se modifica lista)
        /// </summary>
        public async Task<List<PropuestaEstudianteDto>> ActualizarEstudiantesAsync(
            int propuestaId,
            List<int> nuevosEstudianteIds,
            string motivo,
            string? realizadoPor = null)
        {
            if (propuestaId <= 0)
                throw new ArgumentException("ID de propuesta inválido", nameof(propuestaId));

            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("Debe proporcionar un motivo para la actualización", nameof(motivo));

            // Validar propuesta
            var propuesta = await _propuestaRepository.GetByIdAsync(propuestaId);
            if (propuesta == null)
                throw new KeyNotFoundException($"Propuesta con ID {propuestaId} no encontrada");

            // Si hay cambios en estudiantes, se regresa a PENDIENTE
            if (propuesta.Estado == "APROBADA")
            {
                propuesta.Estado = "PENDIENTE";
                propuesta.FechaActualizacion = DateTime.UtcNow;
                await _propuestaRepository.UpdateAsync(propuesta);
            }

            return await AsignarEstudiantesAsync(propuestaId, nuevosEstudianteIds, motivo, realizadoPor);
        }

        /// <summary>
        /// Obtiene el número de estudiantes asignados a una propuesta
        /// </summary>
        public async Task<int> ObtenerConteoEstudiantesAsync(int propuestaId)
        {
            if (propuestaId <= 0)
                throw new ArgumentException("ID de propuesta inválido", nameof(propuestaId));

            return await _propuestaEstudianteRepository.CountActivosByPropuestaIdAsync(propuestaId);
        }

        /// <summary>
        /// Verifica si una propuesta tiene estudiantes asignados
        /// </summary>
        public async Task<bool> TieneEstudiantesAsync(int propuestaId)
        {
            if (propuestaId <= 0)
                throw new ArgumentException("ID de propuesta inválido", nameof(propuestaId));

            return await _propuestaEstudianteRepository.TieneEstudiantesAsync(propuestaId);
        }
    }
}
