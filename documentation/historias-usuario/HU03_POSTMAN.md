# 📤 DOCUMENTACIÓN POSTMAN - HU03 "Validar y enviar propuesta TIC a revisión"

## 🎯 RESUMEN HU03

**Objetivo:** Permitir que los docentes validen y envíen sus propuestas TIC a revisión, asegurando que tengan toda la información requerida.

**Tareas implementadas:**

1. **T07 Backend**: Endpoint POST /api/propuestas/{id}/enviar-revision con validaciones completas
2. **T08 Frontend**: Botón "Enviar a Revisión" con modal de confirmación y validaciones
3. **T09 Validaciones completas**: Validaciones bidireccionales (backend + frontend)

**Endpoints involucrados:**

1. `POST /api/propuestas/{id}/enviar-revision` - Enviar propuesta a revisión (T07)
2. `PATCH /api/propuestas/{id}/estado` - Cambiar estado (antiguo método, aún funciona)

---

## 📋 VALIDACIONES REQUERIDAS

### Validaciones Obligatorias (T09)

Una propuesta SOLO puede enviarse a revisión si cumple TODOS los requisitos:

| Campo               | Validación                     | Ubicación          |
| ------------------- | ------------------------------ | ------------------ |
| **Nombre Proyecto** | 10-250 caracteres              | Frontend + Backend |
| **Participantes**   | Número 1-5                     | Frontend + Backend |
| **Descripción**     | Mínimo 20 caracteres           | Frontend + Backend |
| **Objetivo**        | No vacío, mínimo 20 caracteres | Frontend + Backend |
| **Alcance**         | No vacío, mínimo 20 caracteres | Frontend + Backend |
| **Asignaturas**     | Mínimo 1 asignada              | Frontend + Backend |
| **Estado**          | Debe ser BORRADOR              | Backend            |

### Restricciones Adicionales (HU03 T09)

- ✅ **NO permitir edición**: Si estado es PENDIENTE, OBSERVADA, APROBADA o RECHAZADA
- ✅ **Deshabilitar campos**: Formulario se desactiva automáticamente
- ✅ **Mostrar advertencia**: Mensaje visual indicando estado

---

## 🔧 ENDPOINT T07: Enviar Propuesta a Revisión

**Método:** `POST`
**URL:** `http://localhost:5000/api/propuestas/{id}/enviar-revision`
**Ubicación Backend:** [PropuestasController.cs - EnviarARevision()](backend/TesisTIC.API/Controllers/PropuestasController.cs)

### Headers

```
Content-Type: application/json
```

### Request Body

```json
{}
```

**Nota:** El body puede estar vacío. El servidor valida todos los campos de la propuesta.

### Response (200 OK)

Si todas las validaciones pasan, la propuesta se cambia a estado PENDIENTE:

```json
{
  "id": 2,
  "nombreProyecto": "Aplicación móvil para tutorías estudiantiles",
  "numeroParticipantes": 2,
  "profesorId": 2,
  "descripcion": "Una aplicación móvil que permitirá a los estudiantes...",
  "objetivo": "Desarrollar una plataforma móvil escalable...",
  "alcance": "El sistema cubrirá funcionalidades de...",
  "estado": "PENDIENTE",
  "fechaCreacion": "2026-03-18T14:00:00Z",
  "fechaActualizacion": "2026-04-05T10:30:00Z",
  "fechaEnvioRevision": "2026-04-05T10:30:00Z",
  "profesor": {
    "id": 2,
    "nombres": "María",
    "apellidos": "García",
    "correo": "maria.garcia@epn.edu.ec"
  },
  "asignaturas": [
    {
      "id": 5,
      "codigo": "ISWD713",
      "nombre": "Aplicaciones Móviles"
    }
  ]
}
```

### Posibles Errores

#### 400 Bad Request - Campo Inválido

```json
{
  "message": "El nombre del proyecto debe tener al menos 10 caracteres"
}
```

**Mensajes posibles:**

- `"El nombre del proyecto es requerido"`
- `"El nombre del proyecto debe tener al menos 10 caracteres"`
- `"El nombre del proyecto no puede exceder 250 caracteres"`
- `"El número de participantes debe estar entre 1 y 5"`
- `"La descripción de la propuesta es requerida"`
- `"La descripción debe tener al menos 20 caracteres"`
- `"El objetivo de la propuesta es requerido"`
- `"El alcance de la propuesta es requerido"`
- `"Debe asignar al menos una asignatura a la propuesta"`

#### 400 Bad Request - Estado Inválido

```json
{
  "message": "La propuesta debe estar en estado BORRADOR para enviarla a revisión. Estado actual: PENDIENTE"
}
```

#### 404 Not Found

```json
{
  "message": "Propuesta con ID 999 no encontrada"
}
```

#### 500 Internal Server Error

```json
{
  "message": "Error interno del servidor"
}
```

---

## 🎯 FLUJO HU03: Envío a Revisión

```
1. Usuario abre formulario (edición de propuesta BORRADOR)
   ↓
2. Completa todos los campos:
   - Nombre: "Aplicación móvil..." (10-250 chars)
   - Participantes: 2 (1-5)
   - Descripción: "Una app que permite..." (20+ chars)
   - Objetivo: "Desarrollar una plataforma..." (20+ chars)
   - Alcance: "Cubrirá funcionalidades..." (20+ chars)
   - Asignaturas: Selecciona al menos 1
   ↓
3. Hace click en botón "📤 Enviar a Revisión"
   ↓
4. Frontend valida formulario (T08 - Validaciones)
   ↓
5. Si hay errores, muestra en rojo y mensaje específico
   ↓
6. Si todo válido, muestra modal de confirmación (T08)
   ↓
7. Usuario confirma en modal
   ↓
8. POST /api/propuestas/{id}/enviar-revision (T07)
   ↓
9. Backend valida NUEVAMENTE todos los campos (T07 - T09)
   ↓
10. Si pasa: Cambia estado BORRADOR → PENDIENTE
    Registra FechaEnvioRevision = ahora
    ↓
11. Si falla: Devuelve 400 con mensaje de error específico
    ↓
12. Frontend:
    - Éxito: Deshabilita formulario, muestra "Enviado ✓"
    - Recarga tablero automáticamente
    ↓
13. Propuesta aparece en tablero con estado "PENDIENTE" (azul)
```

---

## 📚 EJEMPLOS CURL

### Caso 1: Envío exitoso

```bash
curl -X POST "http://localhost:5000/api/propuestas/2/enviar-revision" \
  -H "Content-Type: application/json" \
  -d '{}'
```

### Caso 2: Propuesta no cumple validaciones

```bash
curl -X POST "http://localhost:5000/api/propuestas/3/enviar-revision" \
  -H "Content-Type: application/json" \
  -d '{}'
# Respuesta: 400 - "La descripción debe tener al menos 20 caracteres"
```

### Caso 3: Propuesta ya está PENDIENTE

```bash
curl -X POST "http://localhost:5000/api/propuestas/1/enviar-revision" \
  -H "Content-Type: application/json" \
  -d '{}'
# Respuesta: 400 - "La propuesta debe estar en estado BORRADOR para enviarla a revisión"
```

---

## 🧪 CASOS DE PRUEBA POSTMAN

### Test 1: Crear propuesta incompleta

```
1. POST /api/propuestas con datos incompletos
2. Respuesta: Propuesta creada con ID (ej: 5)
3. Estado: BORRADOR
```

### Test 2: Intentar enviar sin completar

```
1. POST /api/propuestas/5/enviar-revision
2. Respuesta: 400 - "La descripción debe tener al menos 20 caracteres"
3. Propuesta sigue en BORRADOR
```

### Test 3: Completar propuesta

```
1. PUT /api/propuestas/5 con todos los campos válidos
2. Respuesta: 200 - Propuesta actualizada
```

### Test 4: Enviar a revisión exitosamente

```
1. POST /api/propuestas/5/enviar-revision
2. Respuesta: 200 - Propuesta con estado=PENDIENTE
3. fechaEnvioRevision establecida a ahora
4. FechaActualizacion actualizada
```

### Test 5: Intentar enviar nuevamente

```
1. POST /api/propuestas/5/enviar-revision
2. Respuesta: 400 - "La propuesta debe estar en estado BORRADOR..."
3. NO se cambia a PENDIENTE dos veces
```

---

## 🎨 INTERFAZ USUARIO (T08)

### Formulario en modo BORRADOR

```
[Todos los campos editable]
[Botón: 💾 Guardar como Borrador]
[Botón: 📤 Enviar a Revisión] ← Verde, habilitado si form válido
```

### Modal de Confirmación

```
┌─────────────────────────────────────┐
│  Confirmar Envío a Revisión         │
├─────────────────────────────────────┤
│ ¿Estás seguro de enviar?            │
│ ⚠️ Una vez enviada, no podrás       │
│    editar los campos                │
│                                     │
│ Proyecto: Aplicación móvil...       │
│ Participantes: 2                    │
│ Asignaturas: 1 seleccionadas        │
├─────────────────────────────────────┤
│ [Cancelar]  [✓ Confirmar Envío]    │
└─────────────────────────────────────┘
```

### Formulario después de envío (estado PENDIENTE)

```
[Todos los campos DESHABILITADOS]
⚠️ Estado actual: PENDIENTE
   Esta propuesta está en revisión y no puede editarse
```

### Campo con Error (T09)

```
┌─ Nombre del Proyecto ───────────────┐
│ [                              ] ✗   │
│ Campo requerido                     │
└─────────────────────────────────────┘
```

---

## 📊 DIFERENCIAS CON ENDPOINTS ANTERIORES

### HU01 vs HU03

| Aspecto                | HU01 POST /api/propuestas | HU03 POST /api/propuestas/{id}/enviar-revision |
| ---------------------- | ------------------------- | ---------------------------------------------- |
| **Crear/Editar**       | Permite cualquier estado  | Solo BORRADOR                                  |
| **Validaciones**       | Mínimas                   | Completas (T09)                                |
| **Cambio estado**      | Crea en BORRADOR          | Cambia a PENDIENTE                             |
| **Asignaturas**        | Opcional                  | Obligatorio mínimo 1                           |
| **FechaEnvioRevision** | null                      | Registrada                                     |

### PATCH vs POST

| Endpoint                           | Método | Uso                         | Validaciones |
| ---------------------------------- | ------ | --------------------------- | ------------ |
| `/propuestas/{id}/estado`          | PATCH  | Cambio genérico de estado   | Mínimas      |
| `/propuestas/{id}/enviar-revision` | POST   | Envío a revisión específico | Completas    |

---

## ⚡ PASOS PARA PROBAR EN POSTMAN

### Paso 1: Importar o crear requests

```
Nombre Collection: HU03 - Validar y Enviar Propuesta
```

### Paso 2: Crear propuesta base (HU01)

```
POST http://localhost:5000/api/propuestas
Body:
{
  "nombreProyecto": "Sistema de tutorías TIC",
  "numeroParticipantes": 2,
  "profesorId": 1,
  "descripcion": "Un sistema completo para gestionar sesiones de tutoría",
  "objetivo": "Facilitar la gestión de tutorías académicas en la institución",
  "alcance": "Aplicable a todos los docentes y estudiantes de la EPN",
  "asignaturaIds": [1, 2]
}
```

Guardar el ID de la respuesta (ej: 10)

### Paso 3: Intentar envío sin validaciones (debe fallar)

```
POST http://localhost:5000/api/propuestas/10/enviar-revision
Body: {}
Esperado: 400 - Mensajes de validación
```

### Paso 4: Completar propuesta

```
PUT http://localhost:5000/api/propuestas/10
Body: (mismo de creación, con campos completos)
```

### Paso 5: Envío exitoso

```
POST http://localhost:5000/api/propuestas/10/enviar-revision
Body: {}
Esperado: 200 - estado=PENDIENTE, fechaEnvioRevision establecida
```

### Paso 6: Verificar no se puede re-enviar

```
POST http://localhost:5000/api/propuestas/10/enviar-revision
Body: {}
Esperado: 400 - "La propuesta debe estar en estado BORRADOR..."
```

### Paso 7: Verificar en tablero

```
GET http://localhost:5000/api/propuestas
Esperado: Propuesta 10 aparece con estado PENDIENTE
```

---

## 🔐 SEGURIDAD Y RESTRICCIONES

### Validaciones Backend (T07 - T09)

✅ No confía en datos del frontend
✅ Valida cada campo nuevamente en el servidor
✅ Rechaza estado que no sea BORRADOR
✅ Registra FechaEnvioRevision automáticamente

### Restricciones Frontend (T08 - T09)

✅ Modal de confirmación previene envíos accidentales
✅ Resalta campos inválidos en rojo
✅ Deshabilita formulario después del envío
✅ Mensajes de error específicos

### Flujo Seguro

```
Frontend Valida ↓
Modal Confirma ↓
Backend Valida Nuevamente ↓
Cambio de Estado
```

---

## 🎯 MÉTRICAS HU03

**Backend (.NET):**

- Líneas de código nuevo: ~70 (EnviarARevisionAsync)
- Validaciones: 7 campos
- Endpoints: 1 nuevo POST
- DTOs: 1 nuevo (EnviarARevisionDto)

**Frontend (Angular):**

- Líneas de código nuevo: ~150 (métodos + HTML + SCSS)
- Modal de confirmación: Implementado
- Validaciones visuales: Campos en rojo con mensajes
- Estados del formulario: 3 (edición, PENDIENTE, enviando)

**Documentación:**

- Este archivo: Guía completa Postman
- Ejemplos curl: 3 casos principales
- Casos de prueba: 5 escenarios

---

## 📱 COMPATIBLE CON:

- ✅ Postman (Desktop/Web)
- ✅ Insomnia
- ✅ Thunder Client
- ✅ curl desde terminal
- ✅ Frontend Angular RxJS Observables

---

## 🔗 REFERENCIAS RÁPIDAS

- **Backend Endpoint:** [PropuestasController.EnviarARevision()](backend/TesisTIC.API/Controllers/PropuestasController.cs)
- **Backend Servicio:** [PropuestaService.EnviarARevisionAsync()](backend/TesisTIC.Application/Services/PropuestaService.cs)
- **Backend DTO:** [EnviarARevisionDto.cs](backend/TesisTIC.Application/DTOs/EnviarARevisionDto.cs)
- **Frontend Servicio:** [propuesta.service.ts - enviarARevision()](frontend/src/app/services/propuesta.service.ts)
- **Frontend Componente:** [formulario-propuesta.component.ts](frontend/src/app/components/formulario-propuesta/formulario-propuesta.component.ts)

---

**HU03 - Validar y Enviar Propuesta TIC**
Fecha: 2026-04-05
Versión: 1.0.0
