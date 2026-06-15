using AutoMapper;
using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Services;

/// <summary>
/// Servicio para operaciones con Estudiantes
/// </summary>
public class EstudianteService : IEstudianteService
{
    private readonly IEstudianteRepository _repository;
    private readonly IMapper _mapper;

    public EstudianteService(IEstudianteRepository repository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<EstudianteDto>> GetAllAsync()
    {
        var estudiantes = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<EstudianteDto>>(estudiantes);
    }

    public async Task<EstudianteDto?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID inválido", nameof(id));

        var estudiante = await _repository.GetByIdAsync(id);
        return estudiante != null ? _mapper.Map<EstudianteDto>(estudiante) : null;
    }

    public async Task<EstudianteDto> CreateAsync(CreateUpdateEstudianteDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var nombreEstudiante = dto.NombresEstudiante.Trim();

        if (string.IsNullOrWhiteSpace(nombreEstudiante))
            throw new ArgumentException("El nombre del estudiante es requerido");

        var estudiante = new Estudiante
        {
            NombresEstudiante = nombreEstudiante
        };

        var creado = await _repository.CreateAsync(estudiante);
        return _mapper.Map<EstudianteDto>(creado);
    }

    public async Task<EstudianteDto> UpdateAsync(int id, CreateUpdateEstudianteDto dto)
    {
        if (id <= 0)
            throw new ArgumentException("ID inválido", nameof(id));
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var estudiante = await _repository.GetByIdAsync(id);
        if (estudiante == null)
            throw new InvalidOperationException($"Estudiante con ID {id} no encontrado");

        estudiante.NombresEstudiante = dto.NombresEstudiante.Trim();

        var actualizado = await _repository.UpdateAsync(estudiante);
        return _mapper.Map<EstudianteDto>(actualizado);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID inválido", nameof(id));

        return await _repository.DeleteAsync(id);
    }
}
