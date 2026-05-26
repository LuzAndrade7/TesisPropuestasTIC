using AutoMapper;
using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Services;

/// <summary>
/// Servicio para operaciones con Asignaturas
/// </summary>
public class AsignaturaService : IAsignaturaService
{
    private readonly IAsignaturaRepository _repository;
    private readonly IMapper _mapper;

    public AsignaturaService(IAsignaturaRepository repository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<AsignaturaDto>> GetAllAsync()
    {
        var asignaturas = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<AsignaturaDto>>(asignaturas);
    }

    public async Task<AsignaturaDto?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID inválido", nameof(id));

        var asignatura = await _repository.GetByIdAsync(id);
        return asignatura != null ? _mapper.Map<AsignaturaDto>(asignatura) : null;
    }

    public async Task<AsignaturaDto> CreateAsync(CreateUpdateAsignaturaDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Codigo) || string.IsNullOrWhiteSpace(dto.Nombre))
            throw new ArgumentException("Código y nombre son requeridos");

        // Verificar que el código sea único
        var existente = await _repository.GetByCodigoAsync(dto.Codigo);
        if (existente != null)
            throw new InvalidOperationException($"Asignatura con código {dto.Codigo} ya existe");

        var asignatura = new Asignatura
        {
            Codigo = dto.Codigo,
            Nombre = dto.Nombre
        };

        var creada = await _repository.CreateAsync(asignatura);
        return _mapper.Map<AsignaturaDto>(creada);
    }

    public async Task<AsignaturaDto> UpdateAsync(int id, CreateUpdateAsignaturaDto dto)
    {
        if (id <= 0)
            throw new ArgumentException("ID inválido", nameof(id));
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var asignatura = await _repository.GetByIdAsync(id);
        if (asignatura == null)
            throw new InvalidOperationException($"Asignatura con ID {id} no encontrada");

        // Si cambió el código, verificar que no exista otro
        if (!asignatura.Codigo.Equals(dto.Codigo, StringComparison.OrdinalIgnoreCase))
        {
            var existente = await _repository.GetByCodigoAsync(dto.Codigo);
            if (existente != null)
                throw new InvalidOperationException($"Código {dto.Codigo} ya está en uso");
        }

        asignatura.Codigo = dto.Codigo;
        asignatura.Nombre = dto.Nombre;

        var actualizada = await _repository.UpdateAsync(asignatura);
        return _mapper.Map<AsignaturaDto>(actualizada);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID inválido", nameof(id));

        return await _repository.DeleteAsync(id);
    }
}
