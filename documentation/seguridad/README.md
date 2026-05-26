# Seguridad - Endpoints Protegidos y Validaciones

## Estado Actual de Seguridad

**Autenticación**: ❌ NO IMPLEMENTADA (Otro Moódulo)  
**Autorización**: ❌ NO IMPLEMENTADA (Otro Moódulo)  
**HTTPS**: ✅ En producción (no en local)  
**CORS**: ✅ Configurado (localhost:4200 en desarrollo)  
**SQL Injection**: ✅ Prevenido (EF Core + LINQ)  
**XSS**: ✅ Prevenido (Angular DomSanitizer)

---

## Endpoints NO Protegidos (Acceso Público)

Actualmente, **TODOS los endpoints son públicos**. No hay autenticación/autorización.

```
GET  /api/propuestas                           ✅ Público
GET  /api/propuestas/{id}                      ✅ Público
GET  /api/propuestas/{id}/detalle              ✅ Público
GET  /api/propuestas?estado=BORRADOR           ✅ Público
GET  /api/asignaturas                          ✅ Público
GET  /api/docentes                             ✅ Público
GET  /api/estudiantes                          ✅ Público
GET  /api/estudiantes/buscar                   ✅ Público
GET  /api/estudiantes/disponibles/{id}         ✅ Público
GET  /api/estudiantes/asignados/{id}           ✅ Público
GET  /api/observaciones/propuesta/{id}         ✅ Público

POST /api/propuestas                           ✅ Público
POST /api/propuestas/{id}/enviar-revision      ✅ Público
POST /api/propuestas/{id}/cambiar-estado       ✅ Público
POST /api/propuestas/{id}/reenviar-despues...  ✅ Público
POST /api/observaciones                        ✅ Público
POST /api/estudiantes/{id}/asignar             ✅ Público
POST /api/propuestas/{id}/solicitar-nueva...   ✅ Público

PUT  /api/propuestas/{id}                      ✅ Público

DELETE /api/propuestas/{id}                    ✅ Público
```

---

## Validaciones a Nivel Aplicación (Backend)

Aunque no hay autenticación, el sistema aplica validaciones de negocio:

### 1. Validaciones por Entrada (400 Bad Request)

```csharp
// En Service.CreateAsync()
if (string.IsNullOrEmpty(dto.Titulo))
    throw new ArgumentException("Título requerido");

if (dto.NumeroParticipantes <= 0)
    throw new ArgumentException("Número de participantes debe ser > 0");

if (dto.AsignaturasIds == null || dto.AsignaturasIds.Count == 0)
    throw new ArgumentException("Debe asignar mínimo 1 asignatura");

if (dto.DocenteId <= 0)
    throw new ArgumentException("ID de docente inválido");
```

**Respuesta 400**:

```json
{
  "statusCode": 400,
  "message": "Título requerido"
}
```

### 2. Validaciones por Estado (403 Forbidden)

```csharp
// En Service.UpdateAsync()
if (propuesta.Estado != "BORRADOR" && propuesta.Estado != "OBSERVADA")
    throw new InvalidOperationException(
        $"No se puede editar propuesta en estado {propuesta.Estado}. " +
        "Solo se pueden editar propuestas en estado BORRADOR u OBSERVADA.");

// En Service.DeleteAsync()
if (propuesta.Estado != "BORRADOR")
    throw new InvalidOperationException(
        $"No se puede eliminar propuesta en estado {propuesta.Estado}. " +
        "Solo se pueden eliminar propuestas en estado BORRADOR.");

// En PropuestaEstudianteService.AsignarEstudiantesAsync()
if (propuesta.Estado != "APROBADA" && propuesta.Estado != "PENDIENTE")
    throw new InvalidOperationException(
        "Solo se pueden asignar estudiantes a propuestas APROBADA o PENDIENTE.");

if (estudianteIds.Count > 5)
    throw new ArgumentException("No se pueden asignar más de 5 estudiantes");
```

**Respuesta 403**:

```json
{
  "statusCode": 403,
  "message": "No se puede editar propuesta en estado PENDIENTE. Solo se pueden editar propuestas en estado BORRADOR u OBSERVADA."
}
```

### 3. Validaciones por Existencia (404 Not Found)

```csharp
var propuesta = await _repository.GetByIdAsync(id);
if (propuesta == null)
    throw new KeyNotFoundException($"Propuesta con ID {id} no encontrada");

var docente = await _docenteRepository.GetByIdAsync(dto.DocenteId);
if (docente == null)
    throw new KeyNotFoundException($"Docente con ID {dto.DocenteId} no encontrado");
```

**Respuesta 404**:

```json
{
  "statusCode": 404,
  "message": "Propuesta con ID 9999 no encontrada"
}
```

---

## Matriz de Operaciones Permitidas por Estado

| Operación                           | BORRADOR | PENDIENTE | OBSERVADA | APROBADA | RECHAZADA |
| ----------------------------------- | -------- | --------- | --------- | -------- | --------- |
| **READ** (Ver detalle)              | ✅       | ✅        | ✅        | ✅       | ✅        |
| **UPDATE** (Editar)                 | ✅       | ❌        | ✅        | ❌       | ❌        |
| **DELETE** (Eliminar)               | ✅       | ❌        | ❌        | ❌       | ❌        |
| **POST enviar-revision**            | ✅       | ❌        | ❌        | ❌       | ❌        |
| **POST reenviar**                   | ❌       | ❌        | ✅        | ❌       | ❌        |
| **POST asignar-estudiantes**        | ❌       | ✅        | ❌        | ✅       | ❌        |
| **POST observaciones**              | ❌       | ✅        | ❌        | ❌       | ❌        |
| **POST solicitar-nueva-aprobacion** | ❌       | ❌        | ❌        | ✅       | ❌        |

---

## Inyección SQL (Protegida)

**Problema**: Ataques SQL injection vía parámetros

**Solución Implementada**: EF Core + LINQ (Parameterized Queries)

```csharp
// ✅ SEGURO - Parámetro automático
var propuesta = await _dbSet
    .Where(p => p.Titulo == titulo)  // Parámetro sanitizado
    .FirstOrDefaultAsync();

// ❌ INSEGURO - Si usara concatenación
string query = $"SELECT * FROM propuestas WHERE titulo = '{titulo}'";
// Atacante: titulo = "'; DROP TABLE propuestas; --"

// EF Core NUNCA concatena, siempre usa parámetros
```

**Resultado**: Imposible inyectar SQL

---

## Cross-Site Scripting (XSS) - Protegida

**Problema**: Inyectar scripts maliciosos en datos

**Solución Implementada**: Angular DomSanitizer + Binding seguro

```typescript
// ✅ SEGURO - Angular escapa automáticamente
<p>{{ propuesta.titulo }}</p>  // Si titulo = "<script>alert('xss')</script>"
                               // Mostrará el texto literal, NO ejecutará

// ❌ INSEGURO - Vinculación directa a HTML
<p [innerHTML]="propuesta.titulo"></p>  // MÁS arriesgado

// ✅ SEGURO - Si necesita HTML
<p [innerHTML]="propuesta.descripcion | sanitizeHtml"></p>
// Con pipe personalizado que valida HTML
```

**Resultado**: Contenido malicioso se muestra como texto, no se ejecuta

---

## CORS (Cross-Origin Resource Sharing)

**Configuración en Backend** (`Program.cs`):

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", builder =>
    {
        builder
            .WithOrigins(
                "http://localhost:4200",      // Frontend local
                "http://localhost:3000"       // Otros servicios
            )
            .AllowAnyMethod()                 // GET, POST, PUT, DELETE
            .AllowAnyHeader()                 // Content-Type, etc
            .AllowCredentials();              // Cookies, auth tokens
    });
});

app.UseCors("DevelopmentPolicy");
```

**Efecto**:

- ✅ Frontend (localhost:4200) puede acceder a API (localhost:5000)
- ❌ Otros dominios no pueden acceder

**En Producción**:

```csharp
.WithOrigins("https://testistic.epn.edu.ec")
```

---

## Validaciones Frontend (Angular)

Complementan validaciones backend (no las reemplazan).

### Reactive Forms Validation

```typescript
this.formGroup = this.fb.group({
  titulo: [
    "",
    [
      Validators.required, // No vacío
      Validators.minLength(3), // Mínimo 3 caracteres
      Validators.maxLength(255), // Máximo 255
    ],
  ],
  numeroParticipantes: [
    "",
    [
      Validators.required,
      Validators.min(1), // Mínimo 1
      Validators.max(10), // Máximo 10
    ],
  ],
  asignaturasIds: [
    "",
    [
      Validators.required,
      this.minArrayLength(1), // Mínimo 1 asignatura
    ],
  ],
});
```

### Validación al Enviar

```typescript
onSave(): void {
  if (!this.formGroup.valid) {
    alert('Por favor, completa todos los campos requeridos');
    return;  // No envía al backend
  }

  // Solo llega al backend si es válido
  this.propuestaService.crear(this.formGroup.value)
    .subscribe(...)
}
```

---

## Rate Limiting (No Implementado - Futuro)

**Planeado para próxima iteración**:

```csharp
builder.Services.AddRateLimiting(options =>
{
    options.AddFixedWindowLimiter(
        policyName: "fixed",
        configure: options =>
        {
            options.PermitLimit = 100;        // 100 requests
            options.Window = TimeSpan.FromMinutes(1);  // por minuto
        });
});
```

---

## Auditoría (Parcialmente Implementada)

### Registros en BD

```sql
-- HISTORIAL_ESTADOS: Todos los cambios de estado
SELECT * FROM historial_estados
WHERE propuesta_id = 42
ORDER BY fecha_cambio DESC;

-- Resultado:
propuesta_id │ estado_anterior │ estado_nuevo │ fecha_cambio          │ usuario
42           │ BORRADOR        │ PENDIENTE    │ 2026-05-25 09:30:00   │ usuario@example.com
42           │ PENDIENTE       │ OBSERVADA    │ 2026-05-26 10:00:00   │ cpgic@example.com
42           │ OBSERVADA       │ PENDIENTE    │ 2026-05-27 14:30:00   │ usuario@example.com
```

### Logs en Backend

```csharp
_logger.LogInformation(
    "✅ HU03 T10: Propuesta {propuestaId} enviada a revisión",
    propuestaId
);

_logger.LogWarning(
    "⚠️ Intento de editar propuesta {propuestaId} en estado {estado}",
    propuestaId, propuesta.Estado
);

_logger.LogError(
    ex, "❌ Error al crear propuesta: {error}",
    ex.Message
);
```

---

## Recomendaciones de Seguridad

### Corto Plazo (Para Producción)

1. **Implementar Autenticación OAuth2**

   ```csharp
   builder.Services.AddAuthentication("Bearer")
       .AddJwtBearer(...);
   app.UseAuthentication();
   ```

2. **Implementar Autorización (Roles)**

   ```csharp
   [Authorize(Roles = "PROFESOR,CPGIC")]
   public async Task<ActionResult> CreateObservation(...)
   ```

3. **Validar Origin en CORS**

   ```csharp
   .WithOrigins("https://tudominio.com")
   ```

4. **Implementar HTTPS**

   ```csharp
   app.UseHttpsRedirection();
   ```

5. **Rate Limiting**
   ```csharp
   app.UseRateLimiter();
   ```

### Mediano Plazo

6. **Logging y Monitoreo**
   - Implementar Serilog o similares
   - Enviar logs a servicio externo (Datadog, ELK)

7. **Secrets Management**
   - Usar Azure Key Vault o AWS Secrets Manager
   - NO guardar secrets en appsettings.json

8. **API Key**
   - Para integraciones futuras
   - Rotación periódica

### Largo Plazo

9. **Compliance**
   - GDPR (si usuarios en EU)
   - LGPD (si usuarios en Brasil)
   - Auditoría externa

10. **Security Scanning**
    - OWASP ZAP
    - SonarQube
    - Snyk (dependencias)

---

## Checklist de Seguridad

```
ACTUAL (Desarrollo):
✅ SQL Injection protegida (EF Core)
✅ XSS protegida (Angular DomSanitizer)
✅ CORS configurado (localhost:4200)
✅ Validaciones de estado
✅ Auditoría de cambios de estado
❌ Autenticación
❌ Autorización / Roles
❌ HTTPS (local OK, prod NO)
❌ Rate Limiting
❌ Secrets management

ANTES DE PRODUCCIÓN:
□ Implementar JWT / OAuth2
□ Configurar HTTPS
□ Implementar Rate Limiting
□ Auditoría externa
□ Secrets en Key Vault
□ CORS solo dominio oficial
```

---

## Referencia de Códigos HTTP de Seguridad

| Código                        | Significado                    | Ejemplo                     |
| ----------------------------- | ------------------------------ | --------------------------- |
| **400 Bad Request**           | Datos inválidos                | Título vacío, ID < 0        |
| **401 Unauthorized**          | No autenticado                 | (Futuro: sin JWT)           |
| **403 Forbidden**             | Autenticado pero no autorizado | Estado no permite operación |
| **404 Not Found**             | Recurso no existe              | Propuesta ID 9999           |
| **429 Too Many Requests**     | Rate limit excedido            | (Futuro)                    |
| **500 Internal Server Error** | Error servidor                 | Excepción no capturada      |

---
