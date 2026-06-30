# HU05: Editar Propuesta TIC - Documentación Completa

## Descripción de la Historia de Usuario

Permitir que los usuarios (docentes/CPGIC) editen propuestas TIC que se encuentren en estado **BORRADOR** u **OBSERVADA**.

### Reglas de Negocio

- ✅ Permitir edición si estado es: **BORRADOR** o **OBSERVADA**
- ❌ NO permitir edición si estado es: **PENDIENTE**, **APROBADA**, **RECHAZADA**
- Actualizar automáticamente `FechaActualizacion` al guardar cambios
- Validaciones flexibles (permitir guardar incompleto en BORRADOR)
- Validaciones estrictas en OBSERVADA si se desea reenviar

---

## Tareas Implementadas

### T14: Backend - Editar Propuesta en BORRADOR u OBSERVADA ✅

#### Endpoint: PUT /api/propuestas/{id}

**Ubicación:** `backend/TesisTIC.API/Controllers/PropuestasController.cs`

```csharp
[HttpPut("{id}")]
public async Task<ActionResult<PropuestaDto>> Update(int id, [FromBody] UpdatePropuestaDto dto)
```

**Validaciones:**

- ✅ Verifica que la propuesta existe
- ✅ Solo permite editar si estado es BORRADOR u OBSERVADA
- ✅ Retorna 403 Forbidden si no se puede editar
- ✅ Validación de campos opcionales
- ✅ Actualiza FechaActualizacion automáticamente

**Respuestas:**

- `200 OK`: Propuesta actualizada exitosamente
- `400 Bad Request`: Validación fallida
- `403 Forbidden`: No se puede editar (estado no permitido)
- `404 Not Found`: Propuesta no existe

**Ejemplo de Request:**

```json
{
  "nombreProyecto": "Sistema Mejorado de Gestión",
  "numeroParticipantes": 4,
  "descripcion": "Descripción actualizada",
  "objetivo": "Objetivo actualizado",
  "alcance": "Alcance actualizado",
  "asignaturaIds": [1, 2, 3]
}
```

**Ejemplo de Response (200 OK):**

```json
{
  "id": 1,
  "nombreProyecto": "Sistema Mejorado de Gestión",
  "numeroParticipantes": 4,
  "profesorId": 1,
  "descripcion": "Descripción actualizada",
  "objetivo": "Objetivo actualizado",
  "alcance": "Alcance actualizado",
  "estado": "BORRADOR",
  "fechaCreacion": "2026-05-18T10:00:00Z",
  "fechaActualizacion": "2026-05-18T15:30:00Z",
  "profesor": {
    "id": 1,
    "nombres": "Juan",
    "apellidos": "Pérez"
  },
  "asignaturas": [{ "id": 1, "codigo": "TIC101", "nombre": "Fundamentos" }]
}
```

#### Cambios en el Backend

**Archivo Modificado:** `TesisTIC.Application/Services/PropuestaService.cs`

```csharp
public async Task<PropuestaDto> UpdateAsync(int id, UpdatePropuestaDto dto)
{
    // HU05 T14: Validar que solo se edita en estado BORRADOR u OBSERVADA
    if (propuesta.Estado != "BORRADOR" && propuesta.Estado != "OBSERVADA")
        throw new InvalidOperationException(
            $"No se puede editar una propuesta en estado '{propuesta.Estado}'. " +
            $"Solo se pueden editar propuestas en estado BORRADOR u OBSERVADA.");

    // Actualización parcial: solo campos no null
    // Manejo de relaciones con asignaturas
    // Actualiza FechaActualizacion automáticamente
}
```

---

### T15: Frontend - Vista de Edición ✅

#### Componente: FormularioPropuestaComponent

**Ubicación:** `frontend/src/app/components/formulario-propuesta/formulario-propuesta.component.ts`

**Cambios:**

1. ✅ Propiedades para controlar edición:
   - `puedeEditar: boolean` - Indica si se puede editar
   - `noEditable: boolean` - Indica si NO se puede editar
   - `estadoPropuesta: string` - Estado actual

2. ✅ Método actualizado `cargarPropuesta()`:
   - Permite edición en BORRADOR y OBSERVADA
   - Deshabilita formulario si no se puede editar
   - Muestra mensaje de advertencia

3. ✅ Método mejorado `guardarBorrador()`:
   - Diferencia entre crear y actualizar
   - Muestra mensaje apropiado
   - Reutiliza `PropuestaService.actualizarPropuesta()`

**Ejemplo de flujo:**

```
1. Usuario navega a /propuestas/123
2. Component carga propuesta existente
3. Si estado es BORRADOR u OBSERVADA:
   - Habilita formulario
   - Muestra datos existentes
   - Permite editar
4. Si estado es PENDIENTE/APROBADA/RECHAZADA:
   - Deshabilita formulario
   - Muestra alerta de bloqueo
```

#### Template Actualizado

**Ubicación:** `frontend/src/app/components/formulario-propuesta/formulario-propuesta.component.html`

**Nuevos elementos:**

- Alerta `alerta-no-editable` cuando estado no permite edición
- Icono de candado (🔒)
- Mensaje informativo sobre estados editables

#### Estilos Nuevos

**Ubicación:** `frontend/src/app/components/formulario-propuesta/formulario-propuesta.component.scss`

```scss
.alerta-no-editable {
  background-color: #fff3cd;
  color: #856404;
  border: 1px solid #ffc107;
}
```

---

### T16: Conexión Frontend-Backend ✅

#### Flujo de Actualización:

1. **Carga de datos** → `formulario-propuesta.component.ts: cargarPropuesta()`
   - GET `/api/propuestas/{id}` vía `PropuestaService`
2. **Edición del formulario** → `FormGroup` reactivo
   - Validaciones locales
   - Actualización de vista

3. **Guardado** → `formulario-propuesta.component.ts: guardarBorrador()`
   - PUT `/api/propuestas/{id}` vía `PropuestaService.actualizarPropuesta()`
   - Manejo de errores (400, 403, 500)
   - Mensaje de éxito/error

4. **Redirección** → `/tablero`
   - Actualización automática del tablero
   - Refleja cambios en propuesta

#### Servicio Utilizado

**Ubicación:** `frontend/src/app/services/propuesta.service.ts`

```typescript
actualizarPropuesta(id: number, propuesta: UpdatePropuestaRequest): Observable<Propuesta> {
  return this.http.put<Propuesta>(`${this.apiUrl}/${id}`, propuesta)
    .pipe(catchError(this.handleError))
}
```

---

## Archivos Modificados Resumen

### Backend

- ✅ `TesisTIC.Application/Services/PropuestaService.cs` - Validación de estado
- ✅ `TesisTIC.API/Controllers/PropuestasController.cs` - Manejo de errores mejorado

### Frontend

- ✅ `formulario-propuesta.component.ts` - Lógica de edición
- ✅ `formulario-propuesta.component.html` - Alerta de bloqueo
- ✅ `formulario-propuesta.component.scss` - Estilos nuevos

---

## Validaciones Implementadas

### Backend

| Validación          | Error           | Código                      |
| ------------------- | --------------- | --------------------------- |
| Propuesta no existe | 404 Not Found   | `InvalidOperationException` |
| Estado no editable  | 403 Forbidden   | `InvalidOperationException` |
| Campos inválidos    | 400 Bad Request | `ArgumentException`         |
| Asignatura vacía    | 400 Bad Request | `ArgumentException`         |

### Frontend

| Validación         | Comportamiento                  |
| ------------------ | ------------------------------- |
| Estado no editable | Deshabilita formulario + Alerta |
| Cambios sin enviar | Mensaje de éxito temporal       |
| Error del servidor | Mensaje de error persistente    |

---

## Flujos de Uso

### Caso 1: Editar propuesta en BORRADOR

```
1. Usuario abre /propuestas/123 (estado BORRADOR)
2. Componente carga datos existentes
3. Usuario modifica campos
4. Hace clic en "Guardar cambios"
5. PUT /api/propuestas/123 → 200 OK
6. Mensaje: "✓ Cambios guardados exitosamente"
7. Redirección a /tablero
```

### Caso 2: Editar propuesta en OBSERVADA

```
1. Usuario abre /propuestas/456 (estado OBSERVADA, con observaciones)
2. Componente carga datos + alerta de observaciones (HU04)
3. Usuario corrige campos especificados
4. Guarda cambios
5. PUT /api/propuestas/456 → 200 OK
6. Usuario luego puede reenviar (HU04 T12)
```

### Caso 3: Intenta editar propuesta PENDIENTE

```
1. Usuario abre /propuestas/789 (estado PENDIENTE)
2. Componente carga datos
3. Muestra alerta: "🔒 Esta propuesta está en estado PENDIENTE..."
4. Formulario deshabilitado
5. Usuario ve botones deshabilitados
```

---

## Testing - Comandos cURL

### Actualizar propuesta en BORRADOR

```bash
curl -X PUT http://localhost:5000/api/propuestas/1 \
  -H "Content-Type: application/json" \
  -d '{
    "nombreProyecto": "Propuesta Actualizada",
    "numeroParticipantes": 4,
    "descripcion": "Nueva descripción",
    "objetivo": "Nuevo objetivo",
    "alcance": "Nuevo alcance",
    "asignaturaIds": [1, 2]
  }'
```

**Respuesta esperada:** 200 OK con propuesta actualizada

### Intenta actualizar propuesta PENDIENTE

```bash
curl -X PUT http://localhost:5000/api/propuestas/2 \
  -H "Content-Type: application/json" \
  -d '{"nombreProyecto": "Test"}'
```

**Respuesta esperada:** 403 Forbidden

```json
{
  "message": "No se puede editar una propuesta en estado 'PENDIENTE'. Solo se pueden editar propuestas en estado BORRADOR u OBSERVADA."
}
```

---

## Integración con HU03 y HU04

- **HU03:** Después de enviar a revisión, el estado cambia a PENDIENTE
  - Ya no se puede editar con HU05
  - Botón "Enviar a Revisión" deshabilitado

- **HU04:** Si CPGIC agrega observaciones:
  - Estado cambia a OBSERVADA
  - HU05 permite editar nuevamente
  - Usuario corrige y puede reenviar (HU04 T12)

---

## Estados Permitidos para Edición

```
BORRADOR ────────────────────────► Editable ✓
         ↓ (enviar a revisión - HU03)
       PENDIENTE ────────────────► NO Editable ✗
         ↓ (agregar observaciones - HU04)
       OBSERVADA ────────────────► Editable ✓ (HU05)
         ↓ (reenviar - HU04 T12)
       PENDIENTE ────────────────► NO Editable ✗
         ↓
       APROBADA ─────────────────► NO Editable ✗
         ↓
       RECHAZADA ────────────────► NO Editable ✗
```

---

## Proximas Tareas Relacionadas

- [ ] HU04 T12: Reenviar propuesta después de observaciones
- [ ] HU04 T11: Visualizar observaciones en formulario
- [ ] HU06: Cambio de docente responsable
- [ ] Reportes de propuestas modificadas

---

**Última actualización:** 18 Mayo 2026
**Versión:** 1.0
**Estado:** ✅ COMPLETA
