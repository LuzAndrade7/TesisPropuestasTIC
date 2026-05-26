# 📘 DOCUMENTACIÓN POSTMAN - HU01 "Registrar y guardar propuesta TIC"

## 🎯 RESUMEN HU01

**Objetivo:** Permitir que un profesor registre y guarde una propuesta TIC como borrador o envíe a revisión.

**Endpoints involucrados:**

1. `POST /api/propuestas` - Crear/guardar propuesta
2. `GET /api/asignaturas` - Obtener asignaturas para dropdown
3. `GET /api/docentes` - Obtener docentes
4. `PATCH /api/propuestas/{id}/estado` - Cambiar estado a PENDIENTE (envío)

---

## 📋 CASOS DE USO

### Caso 1: Guardar como Borrador (T01)

```
[Frontend] → [Angular FormularioPropuestaComponent]
  → [PropuestaService.guardarBorrador()]
  → [POST /api/propuestas]
  → [Backend PropuestasController.Create()]
  → Estado guardado: BORRADOR
```

### Caso 2: Enviar a Revisión (T03)

```
[Frontend] → [Enviar propuesta]
  → [PropuestaService.cambiarEstado(id, 'PENDIENTE')]
  → [PATCH /api/propuestas/{id}/estado]
  → Estado guardado: PENDIENTE
```

---

## 🔧 ENDPOINT 1: Crear Propuesta (Guardar Borrador)

**Método:** `POST`
**URL:** `http://localhost:5000/api/propuestas`
**Ubicación Backend:** [PropuestasController.cs](c:\Users\USER\source\repos\TesisPropuestasTIC-1\backend\TesisTIC.API\Controllers\PropuestasController.cs#L117)

### Headers

```
Content-Type: application/json
```

### Body (JSON)

```json
{
  "nombreProyecto": "Sistema de Gestión de Propuestas TIC",
  "numeroParticipantes": 3,
  "profesorId": 1,
  "descripcion": "Un sistema integral para la gestión de propuestas TIC en la Escuela Politécnica Nacional. Permite a docentes registrar, enviar a revisión y seguimiento de propuestas de proyectos de software.",
  "objetivo": "Desarrollar una plataforma web que facilite el proceso de registro y evaluación de propuestas TIC, mejorando la eficiencia administrativa y permitiendo una mejor comunicación entre docentes y la comisión evaluadora.",
  "alcance": "El sistema será aplicable a toda la Facultad de Ingeniería, permitiendo gestionar propuestas por diferentes departamentos y niveles académicos.",
  "asignaturaIds": [1, 2, 3]
}
```

### Response (201 Created)

```json
{
  "id": 1,
  "nombreProyecto": "Sistema de Gestión de Propuestas TIC",
  "numeroParticipantes": 3,
  "profesorId": 1,
  "descripcion": "Un sistema integral para la gestión de propuestas TIC...",
  "objetivo": "Desarrollar una plataforma web...",
  "alcance": "El sistema será aplicable...",
  "estado": "BORRADOR",
  "fechaCreacion": "2026-05-17T15:30:00Z",
  "fechaActualizacion": "2026-05-17T15:30:00Z",
  "fechaEnvioRevision": null,
  "profesor": {
    "id": 1,
    "nombres": "Juan",
    "apellidos": "Pérez",
    "correo": "juan.perez@epn.edu.ec",
    "tituloAcademico": "Ingeniero en Sistemas"
  },
  "asignaturas": [
    {
      "id": 1,
      "codigo": "ISTG2001",
      "nombre": "Ingeniería de Software I"
    },
    {
      "id": 2,
      "codigo": "ISTG2002",
      "nombre": "Ingeniería de Software II"
    },
    {
      "id": 3,
      "codigo": "ISTG2003",
      "nombre": "Ingeniería de Software III"
    }
  ]
}
```

### Validaciones (T01)

- ✅ `nombreProyecto`: requerido, 10-250 caracteres
- ✅ `numeroParticipantes`: 1-5 participantes (máximo 5 - T01)
- ✅ `profesorId`: requerido, debe existir
- ✅ `descripcion`: requerido, mínimo 20 caracteres
- ✅ `objetivo`: requerido, mínimo 20 caracteres
- ✅ `alcance`: requerido, mínimo 20 caracteres
- ✅ `asignaturaIds`: requerido, mínimo 1

### Estados HTTP

- **201 Created:** Propuesta creada exitosamente
- **400 Bad Request:** Validación fallida
- **500 Internal Server Error:** Error del servidor

---

## 🔧 ENDPOINT 2: Cambiar Estado a PENDIENTE (Enviar a Revisión)

**Método:** `PATCH`
**URL:** `http://localhost:5000/api/propuestas/{id}/estado`
**Ubicación Backend:** [PropuestasController.cs](c:\Users\USER\source\repos\TesisPropuestasTIC-1\backend\TesisTIC.API\Controllers\PropuestasController.cs#L234)

### Headers

```
Content-Type: application/json
```

### URL Parámetros

- `id`: ID de la propuesta (ej: 1)

### Body (JSON)

```json
{
  "estado": "PENDIENTE"
}
```

### Response (200 OK)

```json
{
  "id": 1,
  "nombreProyecto": "Sistema de Gestión de Propuestas TIC",
  "numeroParticipantes": 3,
  "profesorId": 1,
  "descripcion": "Un sistema integral para la gestión de propuestas TIC...",
  "objetivo": "Desarrollar una plataforma web...",
  "alcance": "El sistema será aplicable...",
  "estado": "PENDIENTE",
  "fechaCreacion": "2026-05-17T15:30:00Z",
  "fechaActualizacion": "2026-05-17T15:35:00Z",
  "fechaEnvioRevision": "2026-05-17T15:35:00Z",
  "profesor": { ... },
  "asignaturas": [ ... ]
}
```

---

## 🔧 ENDPOINT 3: Obtener Asignaturas (para Dropdown)

**Método:** `GET`
**URL:** `http://localhost:5000/api/asignaturas`

### Response (200 OK)

```json
[
  {
    "id": 1,
    "codigo": "ISTG2001",
    "nombre": "Ingeniería de Software I"
  },
  {
    "id": 2,
    "codigo": "ISTG2002",
    "nombre": "Ingeniería de Software II"
  },
  {
    "id": 3,
    "codigo": "ISTG2003",
    "nombre": "Ingeniería de Software III"
  }
]
```

---

## 🔧 ENDPOINT 4: Obtener Docentes

**Método:** `GET`
**URL:** `http://localhost:5000/api/docentes`

### Response (200 OK)

```json
[
  {
    "id": 1,
    "nombres": "Juan",
    "apellidos": "Pérez",
    "correo": "juan.perez@epn.edu.ec",
    "tituloAcademico": "Ingeniero en Sistemas",
    "fechaCreacion": "2026-05-17T10:00:00Z"
  },
  {
    "id": 2,
    "nombres": "María",
    "apellidos": "García",
    "correo": "maria.garcia@epn.edu.ec",
    "tituloAcademico": "Doctora en Informática",
    "fechaCreacion": "2026-05-17T10:00:00Z"
  }
]
```

---

## 🚀 EJEMPLOS CON CURL

### Crear Propuesta (Guardar Borrador)

```bash
curl -X POST "http://localhost:5000/api/propuestas" \
  -H "Content-Type: application/json" \
  -d '{
    "nombreProyecto": "Sistema de Gestión de Propuestas TIC",
    "numeroParticipantes": 3,
    "profesorId": 1,
    "descripcion": "Un sistema integral para la gestión de propuestas TIC en la Escuela Politécnica Nacional.",
    "objetivo": "Desarrollar una plataforma web que facilite el proceso de registro y evaluación de propuestas TIC.",
    "alcance": "El sistema será aplicable a toda la Facultad de Ingeniería.",
    "asignaturaIds": [1, 2]
  }'
```

### Enviar a Revisión (Cambiar Estado)

```bash
curl -X PATCH "http://localhost:5000/api/propuestas/1/estado" \
  -H "Content-Type: application/json" \
  -d '{
    "estado": "PENDIENTE"
  }'
```

### Obtener Asignaturas

```bash
curl "http://localhost:5000/api/propuestas"
```

---

## 📊 FLUJO COMPLETO HU01

```
1. Usuario abre formulario
   ↓
2. Sistema carga asignaturas (GET /asignaturas)
3. Sistema carga docentes (GET /docentes)
   ↓
4. Usuario completa formulario y hace clic "Guardar Borrador"
   ↓
5. Validación en Frontend (Angular Reactive Forms)
   ↓
6. POST /api/propuestas → Backend crea propuesta con estado BORRADOR
   ↓
7. Propuesta guardada (ID: 1)
   ↓
8. Usuario puede:
   a) Editar el borrador
   b) Hacer clic "Enviar a Revisión"
      → PATCH /api/propuestas/1/estado {estado: "PENDIENTE"}
      → Propuesta enviada a revisión
```

---

## ⚡ PASOS PARA PROBAR EN POSTMAN

### Paso 1: Crear una Nueva Collection

1. Abrir Postman
2. Click en **New → Collection**
3. Nombre: `HU01 - Registrar Propuesta TIC`
4. Crear

### Paso 2: Agregar Requests

#### Request 1: Obtener Docentes

```
Método: GET
URL: http://localhost:5000/api/docentes
```

#### Request 2: Obtener Asignaturas

```
Método: GET
URL: http://localhost:5000/api/asignaturas
```

#### Request 3: Crear Propuesta (Guardar Borrador)

```
Método: POST
URL: http://localhost:5000/api/propuestas
Headers: Content-Type: application/json
Body: (raw JSON)
{
  "nombreProyecto": "Mi Propuesta TIC",
  "numeroParticipantes": 2,
  "profesorId": 1,
  "descripcion": "Esta es una descripción de mi propuesta para el proyecto",
  "objetivo": "El objetivo principal es desarrollar un software innovador",
  "alcance": "El alcance incluye todas las fases del desarrollo de software",
  "asignaturaIds": [1, 2]
}
```

#### Request 4: Enviar a Revisión

```
Método: PATCH
URL: http://localhost:5000/api/propuestas/1/estado
Headers: Content-Type: application/json
Body: (raw JSON)
{
  "estado": "PENDIENTE"
}
```

### Paso 3: Ejecutar Requests en Orden

1. Obtener Docentes → Copiar un ID
2. Obtener Asignaturas → Copiar IDs
3. Crear Propuesta → Usar IDs del paso 1 y 2
4. Enviar a Revisión → Usar ID de la respuesta del paso 3

---

## 🔐 MANEJO DE ERRORES

### Error 400: Validación fallida

```json
{
  "message": "Número de participantes debe ser máximo 5"
}
```

### Error 404: Propuesta no encontrada

```json
{
  "message": "Propuesta con ID 999 no encontrada"
}
```

### Error 500: Error del servidor

```json
{
  "message": "Error interno del servidor"
}
```

---

## 📱 INTEGRACIÓN FRONTEND (T03)

El frontend Angular utiliza `PropuestaService` que consume estos endpoints:

**Ubicación:** [src/app/services/propuesta.service.ts](c:\Users\USER\source\repos\TesisPropuestasTIC-1\frontend\src\app\services\propuesta.service.ts)

```typescript
// Guardar como borrador (T01)
propuestaService.guardarBorrador(datos).subscribe({
  next: (propuesta) => console.log("Guardado:", propuesta),
  error: (error) => console.error("Error:", error),
});

// Enviar a revisión (T03)
propuestaService.cambiarEstado(id, "PENDIENTE").subscribe({
  next: (propuesta) => console.log("Enviado:", propuesta),
  error: (error) => console.error("Error:", error),
});
```

---

## 📊 RESTRICCIONES Y REGLAS (T01)

✅ **Guardar estado BORRADOR por defecto**

- Cada nueva propuesta se crea con `estado: "BORRADOR"`

✅ **Permitir guardar incompleto**

- El formulario Angular requiere validaciones mínimas
- El backend también valida

✅ **Máximo 5 participantes**

- Validación en Frontend (HTML `max="5"`)
- Validación en Backend (`Validators.max(5)`)

✅ **Async/Await**

- Backend: `async Task<>`
- Frontend: `Observable<>` con `subscribe()`

---

## 📌 NOTAS IMPORTANTES

1. **Estados válidos:** `BORRADOR`, `PENDIENTE`, `OBSERVADA`, `APROBADA`, `RECHAZADA`
2. **Tabla de BD:** `propuestas`, `propuesta_asignaturas` (NO crear nuevas)
3. **Asignaturas:** Relación Many-to-Many a través de `propuesta_asignaturas`
4. **Docentes:** Solo lectura (GET), no crear desde HU01

---

## 🔗 REFERENCIAS RÁPIDAS

- **Backend Controller:** [PropuestasController.cs](c:\Users\USER\source\repos\TesisPropuestasTIC-1\backend\TesisTIC.API\Controllers\PropuestasController.cs)
- **Backend Service:** [PropuestaService.cs](c:\Users\USER\source\repos\TesisPropuestasTIC-1\backend\TesisTIC.Application\Services\PropuestaService.cs)
- **Frontend Service:** [propuesta.service.ts](c:\Users\USER\source\repos\TesisPropuestasTIC-1\frontend\src\app\services\propuesta.service.ts)
- **Frontend Component:** [formulario-propuesta.component.ts](c:\Users\USER\source\repos\TesisPropuestasTIC-1\frontend\src\app\components\formulario-propuesta\formulario-propuesta.component.ts)
