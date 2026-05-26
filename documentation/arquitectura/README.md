# Arquitectura del Sistema TesisTIC

## Resumen Ejecutivo

El sistema TesisTIC utiliza una **arquitectura por capas (Clean Architecture)** que separa responsabilidades entre presentación, lógica de negocio, modelos de dominio e infraestructura. Esta arquitectura permite mantenibilidad, testabilidad y escalabilidad.

---

## Diagrama Arquitectura (Textual)

```
┌──────────────────────────────────────────────────────────────┐
│                          PRESENTACIÓN                        │
│                                                              │
│  ┌─────────────────────┐      ┌─────────────────────┐      │
│  │  Frontend Angular   │      │   API REST          │      │
│  │  (Port 4200)        │─────→│   Swagger/OpenAPI   │      │
│  │                     │      │   (Port 5000)       │      │
│  └─────────────────────┘      └─────────────────────┘      │
└──────────────────┬─────────────────────────────────────────┘
                   │ HTTP/JSON
┌──────────────────▼─────────────────────────────────────────┐
│                  CAPA API (Controllers)                    │
│                                                             │
│  PropuestasController  EstudiantesController               │
│  AsignaturasController DocentesController                  │
│  ObservacionesController HistorialEstadosController        │
└──────────────────┬──────────────────────────────────────────┘
                   │ Inyección de Dependencias
┌──────────────────▼──────────────────────────────────────────┐
│            CAPA APPLICATION (Services)                      │
│                                                              │
│  PropuestaService          PropuestaEstudianteService      │
│  ObservacionService        HistorialEstadoService          │
│                                                              │
│  Interfaces (Contracts):                                    │
│  - IPropuestaRepository, IEstudianteRepository             │
│  - IPropuestaService, IEstudianteService                   │
│  - AutoMapper para transformación de DTOs                  │
└──────────────────┬───────────────────────────────────────────┘
                   │ Implementación de Interfaces
┌──────────────────▼───────────────────────────────────────────┐
│            CAPA INFRASTRUCTURE (Repositories)               │
│                                                              │
│  GenericRepository<T>        PropuestaRepository           │
│  EstudianteRepository        PropuestaEstudianteRepository │
│  ObservacionRepository       HistorialEstadoRepository     │
│                                                              │
│  Entity Framework Core DbContext: TesisTicDbContext         │
└──────────────────┬───────────────────────────────────────────┘
                   │ LINQ to SQL
┌──────────────────▼───────────────────────────────────────────┐
│              CAPA DOMAIN (Entities)                          │
│                                                              │
│  Propuesta         PropuestaEstudiante   Observacion       │
│  Asignatura        Docente               Estudiante        │
│  HistorialEstado                                            │
│                                                              │
│  Validaciones: Solo lógica de negocio critical             │
└──────────────────┬───────────────────────────────────────────┘
                   │ CRUD Operations
┌──────────────────▼───────────────────────────────────────────┐
│                DATABASE LAYER                               │
│                                                              │
│  PostgreSQL 16 (Neon)                                      │
│  - propuestas                                               │
│  - propuesta_asignaturas (many-to-many)                    │
│  - asignaturas                                              │
│  - docentes                                                 │
│  - estudiantes                                              │
│  - propuesta_estudiantes (many-to-many)                    │
│  - observaciones_cpgic                                      │
│  - historial_estados                                        │
└──────────────────────────────────────────────────────────────┘
```

---

## Flujo de Datos: Ejemplo (Crear Propuesta)

```
1. USUARIO (Frontend)
   └─ Completa formulario y hace click en "Guardar"

2. ANGULAR COMPONENT (detalle-propuesta.component.ts)
   └─ Valida form reactivo localmente
   └─ Llama: propuestaService.crearPropuesta(data)

3. HTTP CLIENT (propuesta.service.ts)
   └─ POST /api/propuestas
   └─ Headers: Content-Type: application/json
   └─ Body: CreatePropuestaRequest (DTO)

4. API CONTROLLER (PropuestasController.cs)
   └─ Recibe POST /api/propuestas/{id}
   └─ Invoca: _service.CreateAsync(dto)
   └─ Mapeo: DTO → Entity via AutoMapper

5. APPLICATION SERVICE (PropuestaService.cs)
   └─ Valida reglas de negocio:
      - Propuesta nueva debe ser BORRADOR
      - Docente debe existir
      - Asignaturas válidas
   └─ Si error: throw ArgumentException (400)
   └─ Si OK: llamar _repository.CreateAsync(entity)

6. INFRASTRUCTURE REPOSITORY (PropuestaRepository.cs)
   └─ GenericRepository.CreateAsync(entity):
      - dbSet.Add(entity)
      - SaveChangesAsync()
      - retorna entity con ID generado

7. DATABASE (PostgreSQL)
   └─ INSERT INTO propuestas (titulo, descripcion, ...)
   └─ RETURNING id (auto-generated)

8. RESPONSE (JSON)
   └─ 201 Created
   └─ Body: PropuestaDto (entidad mapeada)
   └─ Location: /api/propuestas/{id}

9. ANGULAR (propuesta.service.ts)
   └─ catchError captura errores
   └─ Observable<Propuesta> retorna al component

10. COMPONENT (detalle-propuesta.component.ts)
    └─ subscribe({ next: (propuesta) => { ... } })
    └─ Actualiza estado local
    └─ Navega a tablero o muestra success
```

---

## Patrones de Diseño Implementados

### 1. **Repository Pattern**

**Propósito**: Abstraer acceso a datos  
**Ubicación**: `Infrastructure/Repositories/`  
**Interfaz**: `IRepository<T>`, `IReadRepository<T>`, `IWriteRepository<T>`

```csharp
public interface IRepository<T> :
    IReadRepository<T>,       // GetAll, GetById, Count
    IWriteRepository<T>       // Create, Update, Delete, SaveChanges
    where T : class
{
}
```

**Beneficios**:

- Facilita testing (mock repositories)
- Centraliza lógica de acceso a datos
- Permite cambiar BD sin afectar servicios

### 2. **Dependency Injection (DI)**

**Ubicación**: `Program.cs` (ASP.NET Core 8.0 minimal hosting)  
**Contenedor**: IServiceCollection (built-in)

```csharp
// En Program.cs
builder.Services.AddScoped<IPropuestaRepository, PropuestaRepository>();
builder.Services.AddScoped<IPropuestaService, PropuestaService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
```

**Beneficios**:

- Loose coupling entre capas
- Inyección automática en constructores
- Gestión de ciclo de vida (Scoped = por request HTTP)

### 3. **Data Transfer Objects (DTOs)**

**Ubicación**: `Application/DTOs/`  
**Propósito**: Separar modelo de BD del modelo de API

```csharp
// Entity (BD)
public class Propuesta {
    public int Id { get; set; }
    public string Titulo { get; set; }
    public ICollection<PropuestaEstudiante> PropuestaEstudiantes { get; set; }
}

// DTO (API Response)
public class PropuestaDto {
    public int Id { get; set; }
    public string Titulo { get; set; }
    public List<PropuestaEstudianteDto> Estudiantes { get; set; }
}
```

**Beneficios**:

- Expone solo campos necesarios
- Versionamiento de API sin romper BD
- Protege estructuras internas

### 4. **Mapper Pattern (AutoMapper)**

**Ubicación**: `Application/Mappings/`  
**Propósito**: Transformar entities ↔ DTOs automáticamente

```csharp
// En constructor del servicio
var propuestaDto = _mapper.Map<PropuestaDto>(propuestaEntity);
```

**Beneficios**:

- Elimina código repetitivo de transformación
- Mapeos centralizados y reutilizables
- Fácil mantener sincronización entity-DTO

### 5. **Async/Await Pattern**

**Ubicación**: Todo el stack (Controllers, Services, Repositories)  
**Propósito**: No bloquear threads en I/O

```csharp
public async Task<PropuestaDto> CreateAsync(CreatePropuestaRequest dto)
{
    // Validaciones síncronas rápidas
    if (string.IsNullOrEmpty(dto.Titulo))
        throw new ArgumentException("Título requerido");

    // I/O async: BD, filesystem, etc
    var propuesta = new Propuesta { Titulo = dto.Titulo };
    await _repository.CreateAsync(propuesta);  // Espera sin bloquear

    return _mapper.Map<PropuestaDto>(propuesta);
}
```

**Beneficios**:

- Escalabilidad (miles de conexiones simultáneas)
- No desperdicia threads (servidor atiende más usuarios)

### 6. **Exception Handling**

**Ubicación**: Controllers (try-catch)  
**Mapeo**: Exception → HTTP Status Code

```csharp
try {
    await _service.DeleteAsync(id);
    return Ok(new { message = "Eliminado" });
}
catch (KeyNotFoundException)  → 404 Not Found
catch (InvalidOperationException)  → 403 Forbidden
catch (ArgumentException)  → 400 Bad Request
catch (Exception)  → 500 Internal Server Error
```

**Validaciones por nivel**:

- **Controllers**: Captura y mapea a HTTP status codes
- **Services**: Valida reglas de negocio (throw exceptions)
- **Repositories**: Maneja errores de BD

---

## Componentes Principales

### Backend

#### 1. **Propuesta (Núcleo)**

- **Estados**: BORRADOR → PENDIENTE → OBSERVADA/APROBADA/RECHAZADA
- **Relaciones**:
  - 1:N con propuesta_asignaturas
  - 1:N con propuesta_estudiantes
  - N:1 con docentes
  - 1:N con observaciones_cpgic
  - 1:N con historial_estados

#### 2. **PropuestaEstudiante (HU07)**

- **Propósito**: Junction table para muchos-a-muchos Propuesta ↔ Estudiante
- **Campos**: propuesta_id, estudiante_id, fecha_asignacion, asignado_por, estado
- **Limitación**: Máximo 5 estudiantes por propuesta

#### 3. **ObservacionesCPGIC (HU04)**

- **Propósito**: Feedback de la comisión CPGIC
- **Relación**: N:1 con propuestas (una propuesta puede tener varias observaciones)
- **Acción**: Genera cambio de estado PENDIENTE → OBSERVADA

#### 4. **HistorialEstados**

- **Propósito**: Auditoría de cambios de estado
- **Campo**: estado_anterior → estado_nuevo + timestamp
- **Beneficio**: Trazabilidad completa

### Frontend

#### 1. **Tablero Component** (HU02)

- Lista propuestas con filtros por estado
- Botones contextuales según estado

#### 2. **Formulario Propuesta** (HU01, HU05)

- Reactive Forms con validación
- Selector dinámico de asignaturas
- Validación de cambios antes de editar

#### 3. **Detalle Propuesta** (HU06, HU07, HU08)

- Vista completa: datos básicos, profesor, asignaturas
- Observaciones e histórico de estados
- Botones de acción (editar, asignar estudiantes, eliminar, etc)

#### 4. **Modal Asignar Estudiantes** (HU07)

- Search autocomplete con debounce (300ms)
- Chips para estudiantes seleccionados
- Validación máximo 5 estudiantes
- Warning si propuesta está APROBADA

---

## Flujos de Integración

### Frontend ↔ Backend

```
Frontend (Angular 17)          Backend (ASP.NET Core 8)
     │                              │
     ├─ POST /api/propuestas ────→ │ PropuestasController.Create
     │  (CreatePropuestaRequest)    │  → PropuestaService.CreateAsync
     │                              │  → PropuestaRepository.CreateAsync
     │  ← 201 Created ─────────────┤  ← PropuestaDto (DTO)
     │
     ├─ GET /api/propuestas ──────→ │ PropuestasController.GetAll
     │                              │  → PropuestaService.GetAllAsync
     │  ← 200 OK ──────────────────┤  ← List<PropuestaDto>
     │
     ├─ GET /api/propuestas/{id} ─→ │ PropuestasController.GetById
     │  (con includes)              │  → PropuestaService.ObtenerDetalleCompleto
     │  ← 200 OK ──────────────────┤  ← PropuestaDto detallado
     │
     ├─ PUT /api/propuestas/{id} ──→ │ PropuestasController.Update
     │  (UpdatePropuestaRequest)    │  → PropuestaService.UpdateAsync
     │  ← 200 OK ──────────────────┤  ← PropuestaDto actualizado
     │
     ├─ DELETE /api/propuestas/{id}→ │ PropuestasController.Delete
     │  (solo BORRADOR)             │  → PropuestaService.DeleteAsync
     │  ← 200 OK ──────────────────┤  ← { message, propuestaId }
```

### Manejo de Errores

```
Exception Backend             HTTP Status       Frontend Action
─────────────────────────────────────────────────────────────
ArgumentException (null, <0)  400 Bad Request   Mostrar alerta
KeyNotFoundException          404 Not Found     Redirigir tablero
InvalidOperationException     403 Forbidden     Alerta de permisos
Exception (otra)              500 Server Error  Log + alerta genérica
```

---

## Patrones de Comunicación

### Request-Response Síncrono (HTTP)

```json
// Request
POST /api/propuestas
Content-Type: application/json
{
  "titulo": "App Gestión Tareas",
  "descripcion": "...",
  "objetivo": "...",
  "alcance": "...",
  "numeroParticipantes": 3,
  "asignaturasIds": [1, 2],
  "docenteId": 5
}

// Response (201 Created)
Location: /api/propuestas/42
{
  "id": 42,
  "titulo": "App Gestión Tareas",
  "estado": "BORRADOR",
  "fechaCreacion": "2026-05-25T18:30:00Z",
  ...
}
```

---

## Optimizaciones Implementadas

### 1. **Lazy Loading + Eager Loading**

```csharp
// Eager Loading en detalle completo
propuesta = await _dbContext.Propuestas
    .Include(p => p.Asignaturas)
    .Include(p => p.Docente)
    .Include(p => p.PropuestaEstudiantes)
    .ThenInclude(pe => pe.Estudiante)
    .FirstOrDefaultAsync(p => p.Id == id);

// Lazy: en listados simples (sin includes)
propuestas = await _dbSet.ToListAsync();  // Solo propuestas, sin relacionados
```

### 2. **Índices en BD**

```sql
CREATE INDEX idx_propuestas_estado ON propuestas(estado);
CREATE INDEX idx_propuesta_estudiantes_propuesta_id ON propuesta_estudiantes(propuesta_id);
CREATE INDEX idx_propuesta_estudiantes_estudiante_id ON propuesta_estudiantes(estudiante_id);
CREATE INDEX idx_propuesta_estudiantes_composite ON propuesta_estudiantes(propuesta_id, estudiante_id);
```

### 3. **Validación en Capas**

- **Frontend**: Instant feedback (validación reactiva)
- **Backend**: Reglas de negocio críticas
- **BD**: Constraints (UNIQUE, FK, CHECK)

---

## Extensibilidad

### Para Agregar Nueva HU

1. **Crear Entity** en `Domain/Entities/`
2. **Crear DTO** en `Application/DTOs/`
3. **Crear Mapper** en `Application/Mappings/`
4. **Crear Interface** en `Application/Interfaces/`
5. **Implementar Repository** en `Infrastructure/Repositories/`
6. **Implementar Service** en `Application/Services/`
7. **Crear Controller** en `API/Controllers/`
8. **Registrar DI** en `Program.cs`
9. **Crear Component** en Frontend
10. **Actualizar Rutas** en Frontend

---

## Seguridad (Planificada)

- [ ] Autenticación: OAuth2 / JWT
- [ ] Autorización: Roles (Profesor, CPGIC, Admin)
- [ ] CORS: Whitelist de dominios
- [ ] Rate Limiting: Por IP/usuario
- [ ] Validación CSRF: Tokens
- [ ] SQL Injection: Prevented by EF Core LINQ
- [ ] XSS: Angular DomSanitizer

---

## Monitoreo y Logging

**Backend**:

```csharp
_logger.LogInformation("✅ HU01 T01: Propuesta {propuestaId} creada", id);
_logger.LogError(ex, "Error al crear propuesta");
```

**Frontend**:

```typescript
console.log("✅ HU01: Propuesta creada", propuesta);
console.error("❌ Error", error);
```

---

## Conclusiones

La arquitectura de TesisTIC sigue principios SOLID:

- **S**ingle Responsibility: Cada clase tiene una responsabilidad
- **O**pen/Closed: Abierta a extensión, cerrada a modificación
- **L**iskov Substitution: Interfaces bien definidas
- **I**nterface Segregation: DTOs y Interfaces especializados
- **D**ependency Inversion: Inyección de dependencias

Esto resulta en código **mantenible, testeable y escalable**.
