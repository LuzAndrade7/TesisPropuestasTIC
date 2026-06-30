# HU05: Editar Propuesta - Ejemplos Postman

## Configuración Base

- **URL Base:** `http://localhost:5000/api`
- **Content-Type:** `application/json`

---

## T14: Endpoint PUT - Actualizar Propuesta

### 1️⃣ Actualizar propuesta en BORRADOR (Todo correctamente)

**REQUEST:**

```
PUT http://localhost:5000/api/propuestas/1
Content-Type: application/json

{
  "nombreProyecto": "Sistema Avanzado de Gestión TIC",
  "numeroParticipantes": 4,
  "descripcion": "Sistema mejorado con nuevas funcionalidades",
  "objetivo": "Automatizar procesos de gestión",
  "alcance": "Cobertura total de departamentos",
  "asignaturaIds": [1, 2, 3]
}
```

**RESPONSE 200 OK:**

```json
{
  "id": 1,
  "nombreProyecto": "Sistema Avanzado de Gestión TIC",
  "numeroParticipantes": 4,
  "profesorId": 1,
  "descripcion": "Sistema mejorado con nuevas funcionalidades",
  "objetivo": "Automatizar procesos de gestión",
  "alcance": "Cobertura total de departamentos",
  "estado": "BORRADOR",
  "fechaCreacion": "2026-05-18T10:30:00Z",
  "fechaActualizacion": "2026-05-18T15:45:00Z",
  "fechaEnvioRevision": null,
  "profesor": {
    "id": 1,
    "nombres": "Juan",
    "apellidos": "Pérez",
    "correo": "juan.perez@epn.edu.ec"
  },
  "asignaturas": [
    { "id": 1, "codigo": "TIC101", "nombre": "Fundamentos de TIC" },
    { "id": 2, "codigo": "TIC102", "nombre": "Programación" },
    { "id": 3, "codigo": "TIC103", "nombre": "Bases de Datos" }
  ]
}
```

**HTTP Status:** `200 OK` ✅

---

### 2️⃣ Actualización parcial (solo algunos campos)

**REQUEST:**

```
PUT http://localhost:5000/api/propuestas/1
Content-Type: application/json

{
  "descripcion": "Solo actualizo la descripción",
  "numeroParticipantes": 5
}
```

**RESPONSE 200 OK:**

```json
{
  "id": 1,
  "nombreProyecto": "Sistema Avanzado de Gestión TIC",
  "numeroParticipantes": 5,
  "profesorId": 1,
  "descripcion": "Solo actualizo la descripción",
  "objetivo": "Automatizar procesos de gestión",
  "alcance": "Cobertura total de departamentos",
  "estado": "BORRADOR",
  "fechaCreacion": "2026-05-18T10:30:00Z",
  "fechaActualizacion": "2026-05-18T16:00:00Z",
  "profesor": { ... },
  "asignaturas": [...]
}
```

**Nota:** Solo los campos proporcionados se actualizan. Otros mantienen sus valores.

---

### 3️⃣ Actualizar propuesta en OBSERVADA

**REQUEST:**

```
PUT http://localhost:5000/api/propuestas/2
Content-Type: application/json

{
  "descripcion": "Descripción corregida según observaciones de CPGIC",
  "objetivo": "Objetivo mejorado",
  "alcance": "Alcance ampliado"
}
```

**RESPONSE 200 OK:**

```json
{
  "id": 2,
  "nombreProyecto": "Propuesta Original",
  "numeroParticipantes": 3,
  "profesorId": 2,
  "descripcion": "Descripción corregida según observaciones de CPGIC",
  "objetivo": "Objetivo mejorado",
  "alcance": "Alcance ampliado",
  "estado": "OBSERVADA",
  "fechaCreacion": "2026-05-17T09:00:00Z",
  "fechaActualizacion": "2026-05-18T16:15:00Z",
  "fechaEnvioRevision": "2026-05-18T10:00:00Z",
  "profesor": { ... },
  "asignaturas": [...]
}
```

---

### ❌ Errores y Casos de Validación

#### Caso 1: Intenta editar propuesta PENDIENTE

**REQUEST:**

```
PUT http://localhost:5000/api/propuestas/3
Content-Type: application/json

{
  "nombreProyecto": "Intento de edición"
}
```

**RESPONSE 403 Forbidden:**

```json
{
  "message": "No se puede editar una propuesta en estado 'PENDIENTE'. Solo se pueden editar propuestas en estado BORRADOR u OBSERVADA."
}
```

**HTTP Status:** `403 Forbidden` ❌

---

#### Caso 2: Intenta editar propuesta APROBADA

**REQUEST:**

```
PUT http://localhost:5000/api/propuestas/4
Content-Type: application/json

{
  "nombreProyecto": "Cambio no permitido"
}
```

**RESPONSE 403 Forbidden:**

```json
{
  "message": "No se puede editar una propuesta en estado 'APROBADA'. Solo se pueden editar propuestas en estado BORRADOR u OBSERVADA."
}
```

---

#### Caso 3: Propuesta no existe

**REQUEST:**

```
PUT http://localhost:5000/api/propuestas/99999
Content-Type: application/json

{
  "nombreProyecto": "Test"
}
```

**RESPONSE 404 Not Found:**

```json
{
  "message": "Propuesta con ID 99999 no encontrada"
}
```

**HTTP Status:** `404 Not Found` ❌

---

#### Caso 4: Asignaturas vacías

**REQUEST:**

```
PUT http://localhost:5000/api/propuestas/1
Content-Type: application/json

{
  "asignaturaIds": []
}
```

**RESPONSE 400 Bad Request:**

```json
{
  "message": "Debe asignar al menos una asignatura a la propuesta"
}
```

**HTTP Status:** `400 Bad Request` ❌

---

#### Caso 5: Número de participantes inválido

**REQUEST:**

```
PUT http://localhost:5000/api/propuestas/1
Content-Type: application/json

{
  "numeroParticipantes": 0
}
```

**RESPONSE 400 Bad Request:**

```json
{
  "message": "Número de participantes debe ser mayor a 0"
}
```

---

## Flujo Completo de Edición en OBSERVADA

### Paso 1: Cargar propuesta OBSERVADA

```
GET http://localhost:5000/api/propuestas/2

RESPONSE 200 OK:
{
  "id": 2,
  "estado": "OBSERVADA",
  ...
}
```

### Paso 2: Mostrar observaciones (HU04)

```
GET http://localhost:5000/api/observaciones/propuesta/2

RESPONSE 200 OK:
[
  {
    "id": 1,
    "propuestaId": 2,
    "observacion": "Mejorar la descripción del objetivo",
    "realizadoPor": "Dr. Carlos López",
    "fechaObservacion": "2026-05-18T11:00:00Z"
  },
  {
    "id": 2,
    "propuestaId": 2,
    "observacion": "Añadir más detalles en el alcance",
    "realizadoPor": "Dr. Carlos López",
    "fechaObservacion": "2026-05-18T11:05:00Z"
  }
]
```

### Paso 3: Usuario corrige y envía cambios

```
PUT http://localhost:5000/api/propuestas/2

{
  "descripcion": "Descripción mejorada significativamente",
  "objetivo": "Objetivo detallado y claro",
  "alcance": "Alcance expandido con más contexto"
}

RESPONSE 200 OK:
{
  "id": 2,
  "estado": "OBSERVADA",
  "fechaActualizacion": "2026-05-18T16:30:00Z",
  ...
}
```

### Paso 4: Usuario reenvía propuesta (HU04 T12)

```
POST http://localhost:5000/api/propuestas/2/reenviar-despues-observaciones

RESPONSE 200 OK:
{
  "id": 2,
  "estado": "PENDIENTE",
  "fechaActualizacion": "2026-05-18T16:35:00Z",
  ...
}
```

---

## Tabla Resumida de Estados y Edición

| ID  | Propuesta     | Estado    | ¿Editable? | Método | Observaciones           |
| --- | ------------- | --------- | ---------- | ------ | ----------------------- |
| 1   | "Sistema TIC" | BORRADOR  | ✅ Sí      | PUT    | Sin restricciones       |
| 2   | "Propuesta A" | OBSERVADA | ✅ Sí      | PUT    | Pueden reenviar después |
| 3   | "Propuesta B" | PENDIENTE | ❌ No      | -      | Bloqueada en revisión   |
| 4   | "Propuesta C" | APROBADA  | ❌ No      | -      | Aprobada, no editable   |
| 5   | "Propuesta D" | RECHAZADA | ❌ No      | -      | Rechazada, no editable  |

---

## Integración con Fronten (Angular)

```typescript
// Actualizar propuesta
this.propuestaService
  .actualizarPropuesta(propuestaId, {
    nombreProyecto: "Nuevo nombre",
    descripcion: "Nueva descripción",
    asignaturaIds: [1, 2, 3],
  })
  .subscribe({
    next: (propuesta) => {
      console.log("✅ Actualizada:", propuesta);
      this.router.navigate(["/tablero"]);
    },
    error: (error) => {
      if (error.status === 403) {
        console.error("❌ No se puede editar esta propuesta");
      }
    },
  });
```

---

**Versión:** 1.0  
**Última actualización:** 18 Mayo 2026  
**Estado:** ✅ COMPLETA
