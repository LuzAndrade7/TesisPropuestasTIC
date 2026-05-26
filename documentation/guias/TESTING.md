# Guía Completa de Pruebas

## Resumen Ejecutivo

Esta guía proporciona el **orden recomendado** para probar todas las historias de usuario (HU01-HU08), incluye casos de prueba, datos de ejemplo y checklist de validación.

---

## Orden Recomendado de Pruebas

```
┌─ PREPARACIÓN
│  └─ Setup local (Backend + Frontend + BD)
│
├─ HU01: Registro (Crear propuestas en BORRADOR)
│        └─ Datos de prueba creados
│
├─ HU02: Tablero (Listar y filtrar)
│        └─ Usa propuestas de HU01
│
├─ HU03: Envío a Revisión (BORRADOR → PENDIENTE)
│        └─ Usa propuesta de HU01
│
├─ HU04: Observaciones (PENDIENTE → OBSERVADA → PENDIENTE)
│        └─ Ciclo completo de feedback
│
├─ HU05: Edición (Editar BORRADOR u OBSERVADA)
│        └─ Permite correcciones
│
├─ HU06: Detalle Completo (Ver toda info + histórico)
│        └─ Validar datos cargados correctamente
│
├─ HU07: Asignación Estudiantes (APROBADA → asignar → PENDIENTE)
│        └─ Requiere propuesta APROBADA
│
└─ HU08: Eliminación (Eliminar solo BORRADOR)
         └─ Prueba final
```

---

## Pre-requisitos

### Backend

```bash
cd backend
dotnet build                    # Compilar
dotnet run                      # Ejecutar en http://localhost:5000
# En otra terminal:
# Swagger UI disponible en http://localhost:5000/swagger
```

### Frontend

```bash
cd frontend
npm install                     # Una sola vez
npm start                       # Ejecutar en http://localhost:4200
```

### BD

```
PostgreSQL 16 en Neon debe estar accesible
Verificar ConnectionString en appsettings.json
```

### Postman

```
Importar colección: /postman/TesisTIC_Collection.json
Configurar variable: baseUrl = http://localhost:5000/api
```

---

## Datos de Prueba Recomendados

### Docentes (Usar los que ya existen en BD)

| ID | Nombre | Correo | Título |
|----|--------|--------|--------|
| 1 | Dr. Juan Pérez | juan@epn.edu.ec | PhD Ingeniería |
| 2 | Ing. María García | maria@epn.edu.ec | MSc Computación |

### Asignaturas (Usar las que ya existen en BD)

| ID | Código | Nombre | Créditos |
|----|--------|--------|----------|
| 1 | ING101 | Programación I | 4 |
| 2 | ING102 | Bases de Datos | 4 |
| 3 | ING201 | Desarrollo Web | 4 |

### Estudiantes (Serán creados automáticamente en BD)

| ID | Nombre | Correo | Carrera |
|----|--------|--------|---------|
| 1 | Carlos López | carlos@example.com | Ingeniería Sistemas |
| 2 | Ana Rodríguez | ana@example.com | Ingeniería Sistemas |
| 3 | Pedro Martínez | pedro@example.com | Ingeniería Sistemas |

---

## HU01: Registro de Propuestas (T01-T05)

### Objetivo
Crear propuestas en estado BORRADOR con información básica.

### Casos de Prueba

#### ✅ CP1.1 - Crear Propuesta Válida

```json
POST /api/propuestas
{
  "titulo": "App Gestión de Tareas",
  "descripcion": "Aplicación web para gestionar tareas colaborativamente.",
  "objetivo": "Desarrollar plataforma de productividad.",
  "alcance": "Autenticación, asignación, reportes.",
  "numeroParticipantes": 3,
  "asignaturasIds": [1, 2],
  "docenteId": 1
}
```

**Validaciones**:
- [ ] Status 201 Created
- [ ] Response tiene `id`
- [ ] Estado es `BORRADOR`
- [ ] `fechaCreacion` está presente
- [ ] Guardar ID en variable de Postman

**Resultado Esperado**:
```json
{
  "id": 42,
  "titulo": "App Gestión de Tareas",
  "estado": "BORRADOR",
  "fechaCreacion": "2026-05-25T18:30:00Z",
  "docente": { "id": 1, "nombre": "Dr. Juan Pérez", ... },
  "asignaturas": [...]
}
```

#### ❌ CP1.2 - Crear sin Título (Error 400)

```json
POST /api/propuestas
{
  "titulo": "",
  "descripcion": "...",
  ...
}
```

**Validaciones**:
- [ ] Status 400 Bad Request
- [ ] Mensaje de error incluye "título"

#### ❌ CP1.3 - Docente No Existe (Error 404)

```json
POST /api/propuestas
{
  "docenteId": 9999,
  ...
}
```

**Validaciones**:
- [ ] Status 404 Not Found

### Checklist HU01
- [ ] Crear 3 propuestas diferentes
- [ ] Una en español, una en inglés (validar UTF-8)
- [ ] Con diferentes números de participantes (1, 3, 5)
- [ ] Con diferentes combinaciones de asignaturas
- [ ] Verificar que todas quedan en BORRADOR
- [ ] Verificar fechas en formato ISO 8601

---

## HU02: Tablero (T06-T09)

### Objetivo
Listar propuestas filtradas por estado.

### Casos de Prueba

#### ✅ CP2.1 - Listar Todas

```http
GET /api/propuestas
```

**Validaciones**:
- [ ] Status 200 OK
- [ ] Response es Array
- [ ] Mínimo 3 propuestas de HU01

#### ✅ CP2.2 - Filtrar por Estado BORRADOR

```http
GET /api/propuestas?estado=BORRADOR
```

**Validaciones**:
- [ ] Status 200 OK
- [ ] Todas tienen `estado: "BORRADOR"`
- [ ] Solo las de BORRADOR aparecen

#### ✅ CP2.3 - Filtrar por Estado PENDIENTE (Vacío)

```http
GET /api/propuestas?estado=PENDIENTE
```

**Validaciones**:
- [ ] Status 200 OK
- [ ] Array vacío (porque aún no enviamos a revisión)

### Frontend Testing (HU02)

1. Ir a `http://localhost:4200/tablero`
2. Verificar que muestra propuestas de HU01
3. Hacer click en filtro "BORRADOR"
4. Verificar que solo muestra BORRADOR
5. Click en filtro "PENDIENTE" (debe estar vacío)

### Checklist HU02
- [ ] Listar todas retorna >= 3 propuestas
- [ ] Filtro BORRADOR funciona
- [ ] Filtro PENDIENTE retorna vacío
- [ ] Cada propuesta muestra: título, estado, docente
- [ ] Botones de acción visibles (editar, ver detalle, eliminar)

---

## HU03: Envío a Revisión (T10)

### Objetivo
Cambiar estado BORRADOR → PENDIENTE.

### Casos de Prueba

#### ✅ CP3.1 - Enviar a Revisión (Válido)

```http
POST /api/propuestas/{propuestaId}/enviar-revision
```

**Dato**: Usar `propuestaId` de HU01  
**Validaciones**:
- [ ] Status 200 OK
- [ ] Estado cambió a `PENDIENTE`
- [ ] `fechaEnvioRevision` se completó

#### ❌ CP3.2 - Enviar Nuevamente (Error 403)

```http
POST /api/propuestas/{propuestaId}/enviar-revision
```

**Validaciones**:
- [ ] Status 403 Forbidden
- [ ] Mensaje: "No se puede enviar a revisión"

### Frontend Testing (HU03)

1. En detalle de propuesta BORRADOR
2. Click botón "Enviar a Revisión"
3. Confirmar alerta
4. Verificar estado cambió a PENDIENTE
5. Botón debe desaparecer

### Checklist HU03
- [ ] Propuesta cambió a PENDIENTE
- [ ] Fecha de envío se registró
- [ ] No se puede editar después de enviar
- [ ] Aparece en tablero de PENDIENTE
- [ ] Histórico registra el cambio

---

## HU04: Observaciones (T11-T15)

### Objetivo
Ciclo completo de feedback: crear observaciones, cambiar a OBSERVADA, reenviar.

### Casos de Prueba

#### ✅ CP4.1 - Crear Observación

```json
POST /api/observaciones
{
  "propuestaId": {propuestaId},
  "descripcion": "Se requiere aclaración sobre arquitectura técnica.",
  "tipoObservacion": "Técnica"
}
```

**Validaciones**:
- [ ] Status 201 Created
- [ ] Propuesta cambió a `OBSERVADA`
- [ ] Observación tiene fecha

**Nota**: Propuesta debe estar en PENDIENTE

#### ✅ CP4.2 - Listar Observaciones

```http
GET /api/observaciones/propuesta/{propuestaId}
```

**Validaciones**:
- [ ] Status 200 OK
- [ ] Array contiene observación de CP4.1
- [ ] Incluye descripción, tipo, fecha

#### ✅ CP4.3 - Reenviar después de Correcciones

```http
POST /api/propuestas/{propuestaId}/reenviar-despues-observaciones
```

**Validaciones**:
- [ ] Status 200 OK
- [ ] Estado cambió a `PENDIENTE`
- [ ] Observaciones siguen en histórico (no se eliminan)

### Frontend Testing (HU04)

1. Ver propuesta en estado PENDIENTE (de HU03)
2. En detalle, aparece botón "Ver Observaciones" (si las hay)
3. Después de agregar observación, estado es OBSERVADA
4. Profesor puede ir a editar (HU05)
5. Reenviar propuesta
6. Vuelve a PENDIENTE

### Checklist HU04
- [ ] Observación se creó
- [ ] Propuesta cambió a OBSERVADA automáticamente
- [ ] Se pueden crear múltiples observaciones
- [ ] Reenvío limpia estado pero mantiene histórico
- [ ] Profesor puede ver observaciones

---

## HU05: Edición (T16-T17)

### Objetivo
Editar propuestas en BORRADOR u OBSERVADA.

### Casos de Prueba

#### ✅ CP5.1 - Editar BORRADOR

```json
PUT /api/propuestas/{propuestaId}
{
  "titulo": "App Gestión de Tareas - Mejorada",
  "descripcion": "Descripción actualizada...",
  "numeroParticipantes": 4,
  ...
}
```

**Validaciones**:
- [ ] Status 200 OK
- [ ] Cambios aplicados
- [ ] Sigue en BORRADOR

#### ✅ CP5.2 - Editar OBSERVADA

```json
PUT /api/propuestas/{propuestaId}
{
  "descripcion": "Arquitectura técnica aclarada...",
  ...
}
```

**Validaciones**:
- [ ] Status 200 OK
- [ ] Cambios aplicados
- [ ] Sigue en OBSERVADA
- [ ] Fecha actualización se actualiza

#### ❌ CP5.3 - Intentar Editar PENDIENTE (Error 403)

**Validaciones**:
- [ ] Status 403 Forbidden
- [ ] Mensaje: "No puede editarse en este estado"

### Frontend Testing (HU05)

1. Ir a edición de propuesta en BORRADOR
2. Cambiar título
3. Click "Guardar"
4. Verificar cambios se aplicaron
5. Intentar editar propuesta en PENDIENTE
6. Debe mostrar error o deshabilitar form

### Checklist HU05
- [ ] Editar BORRADOR funciona
- [ ] Editar OBSERVADA funciona
- [ ] No se puede editar PENDIENTE, APROBADA, RECHAZADA
- [ ] Cambios se guardan en BD
- [ ] Fecha de actualización se registra

---

## HU06: Detalle Completo (T18-T19)

### Objetivo
Visualizar información completa con profesor, asignaturas, observaciones, estudiantes, histórico.

### Casos de Prueba

#### ✅ CP6.1 - Obtener Detalle Completo

```http
GET /api/propuestas/{propuestaId}/detalle
```

**Validaciones**:
- [ ] Status 200 OK
- [ ] Incluye `profesor` con datos
- [ ] Incluye `asignaturas` array
- [ ] Incluye `observaciones` array
- [ ] Incluye `estudiantesAsignados` array (vacío si no asignados)
- [ ] Incluye `historicoEstados` con transiciones
- [ ] Fechas en formato ISO 8601

**Response Esperado**:
```json
{
  "id": 42,
  "titulo": "...",
  "estado": "OBSERVADA",
  "profesor": {
    "id": 1,
    "nombre": "Dr. Juan Pérez",
    "correo": "juan@epn.edu.ec"
  },
  "asignaturas": [
    { "codigo": "ING101", "nombre": "Programación I", ... }
  ],
  "observaciones": [
    { "descripcion": "...", "fecha": "2026-05-25T..." }
  ],
  "estudiantesAsignados": [],
  "historicoEstados": [
    {
      "estadoAnterior": "BORRADOR",
      "estadoNuevo": "PENDIENTE",
      "fecha": "2026-05-25T18:35:00Z"
    },
    {
      "estadoAnterior": "PENDIENTE",
      "estadoNuevo": "OBSERVADA",
      "fecha": "2026-05-25T18:40:00Z"
    }
  ]
}
```

### Frontend Testing (HU06)

1. Click en propuesta desde tablero
2. Ir a "Ver Detalle"
3. Verificar todas las secciones cargan:
   - [ ] Datos básicos
   - [ ] Profesor info
   - [ ] Asignaturas listadas
   - [ ] Observaciones (si las hay)
   - [ ] Histórico de estados
4. Hacer scroll completo

### Checklist HU06
- [ ] Detalle carga todas las secciones
- [ ] Información de profesor correcta
- [ ] Asignaturas listadas completas
- [ ] Observaciones muestran descripción
- [ ] Histórico de estados completo
- [ ] Estudiantes asignados listos (para HU07)
- [ ] Performance aceptable (< 2s)

---

## HU07: Asignación de Estudiantes (T20-T25)

### Objetivo
Asignar máximo 5 estudiantes a propuesta APROBADA/PENDIENTE.

### Pre-requisito para HU07

La propuesta debe estar en **APROBADA**. Para esto:
1. Crear propuesta (HU01)
2. Enviar a revisión (HU03)
3. NO agregar observaciones
4. Cambiar estado directamente a APROBADA (simulando aprobación CPGIC)

```http
PUT /api/propuestas/{propuestaId}/cambiar-estado
{
  "nuevoEstado": "APROBADA"
}
```

### Casos de Prueba

#### ✅ CP7.1 - Listar Estudiantes

```http
GET /api/estudiantes
```

**Validaciones**:
- [ ] Status 200 OK
- [ ] Array contiene >= 3 estudiantes
- [ ] Cada uno tiene: id, nombre, apellido, correo

#### ✅ CP7.2 - Buscar Estudiante

```http
GET /api/estudiantes/buscar?searchTerm=Juan
```

**Validaciones**:
- [ ] Status 200 OK
- [ ] Retorna estudiantes con "Juan" en nombre/apellido/correo
- [ ] Case-insensitive (busca "juan", "JUAN", "Juan")

#### ✅ CP7.3 - Obtener Disponibles

```http
GET /api/estudiantes/disponibles/{propuestaId}
```

**Validaciones**:
- [ ] Status 200 OK
- [ ] Retorna estudiantes NO asignados a propuesta
- [ ] Array vacío si todos están asignados

#### ✅ CP7.4 - Asignar 3 Estudiantes

```json
POST /api/estudiantes/{propuestaId}/asignar
{
  "estudianteIds": [1, 2, 3],
  "motivo": "Seleccionados por desempeño académico.",
  "realizadoPor": "profesor@example.com"
}
```

**Validaciones**:
- [ ] Status 200 OK
- [ ] Propuesta cambió a `PENDIENTE` (si estaba APROBADA)
- [ ] Retorna lista de estudiantes asignados
- [ ] Cada uno tiene `fechaAsignacion`

#### ❌ CP7.5 - Asignar Más de 5 (Error 400)

```json
POST /api/estudiantes/{propuestaId}/asignar
{
  "estudianteIds": [1, 2, 3, 4, 5, 6],
  ...
}
```

**Validaciones**:
- [ ] Status 400 Bad Request
- [ ] Mensaje: "No se pueden asignar más de 5"

#### ✅ CP7.6 - Solicitar Nueva Aprobación

```json
POST /api/propuestas/{propuestaId}/solicitar-nueva-aprobacion
{
  "motivo": "Se han asignado estudiantes. Se requiere nueva revisión."
}
```

**Validaciones**:
- [ ] Status 200 OK
- [ ] Estado cambió a `PENDIENTE`
- [ ] Histórico registra cambio
- [ ] Motivo guardado

### Frontend Testing (HU07)

1. Ir a propuesta APROBADA en detalle
2. Click botón "Asignar Estudiantes"
3. Modal abre con:
   - [ ] Campo de búsqueda
   - [ ] Dropdown de estudiantes disponibles
   - [ ] Chips de estudiantes seleccionados
   - [ ] Campo "Motivo"
4. Seleccionar 3 estudiantes
5. Click "Asignar"
6. Propuesta debe cambiar a PENDIENTE
7. Aparece botón "Solicitar Nueva Aprobación"
8. Click "Solicitar Nueva Aprobación"
9. Confirmar en alerta
10. Propuesta vuelve a PENDIENTE

### Checklist HU07
- [ ] Búsqueda de estudiantes funciona
- [ ] Se pueden asignar 1-5 estudiantes
- [ ] No permite > 5 estudiantes
- [ ] Propuesta APROBADA → PENDIENTE al asignar
- [ ] Propuesta PENDIENTE con estudiantes
- [ ] Se puede solicitar nueva aprobación
- [ ] Histórico muestra todos los cambios
- [ ] Modal de asignación es responsive

---

## HU08: Eliminación (T26-T28)

### Objetivo
Eliminar propuestas en estado BORRADOR ÚNICAMENTE.

### Casos de Prueba

#### ✅ CP8.1 - Eliminar BORRADOR

```http
DELETE /api/propuestas/{propuestaId}
```

**Datos**: Usar propuesta nueva en BORRADOR (crear con HU01)  
**Validaciones**:
- [ ] Status 200 OK
- [ ] Response: `{ message: "Propuesta eliminada...", propuestaId: 42 }`
- [ ] Propuesta ya no existe en BD
- [ ] GET /api/propuestas/{id} retorna 404

#### ✅ CP8.2 - Verificar Cascada

Después de eliminar propuesta:

```http
GET /api/propuesta_asignaturas?propuesta_id={propuestaId}
GET /api/propuesta_estudiantes?propuesta_id={propuestaId}
GET /api/observaciones/propuesta/{propuestaId}
GET /api/historial_estados?propuesta_id={propuestaId}
```

**Validaciones**:
- [ ] Todos retornan arrays vacíos
- [ ] Cascada eliminó automáticamente datos relacionados

#### ❌ CP8.3 - Intentar Eliminar PENDIENTE (Error 403)

```http
DELETE /api/propuestas/{propuestaId}
```

**Datos**: Usar propuesta en PENDIENTE  
**Validaciones**:
- [ ] Status 403 Forbidden
- [ ] Mensaje: "Solo se pueden eliminar propuestas en BORRADOR"

#### ❌ CP8.4 - Intentar Eliminar APROBADA (Error 403)

**Validaciones**:
- [ ] Status 403 Forbidden

#### ❌ CP8.5 - Eliminar ID No Existe (Error 404)

```http
DELETE /api/propuestas/99999
```

**Validaciones**:
- [ ] Status 404 Not Found

### Frontend Testing (HU08)

1. Crear propuesta nueva (HU01) - quedará en BORRADOR
2. Ir a detalle
3. Click botón "Eliminar Propuesta" (solo si BORRADOR)
4. Modal de confirmación muestra:
   - [ ] Nombre de propuesta
   - [ ] Estado: BORRADOR
   - [ ] Advertencia: "No se puede deshacer"
5. Click confirmar
6. Propuesta eliminada
7. Redirige a tablero
8. Propuesta ya no aparece en listado

### Checklist HU08
- [ ] Botón eliminar solo visible para BORRADOR
- [ ] Confirmación antes de eliminar
- [ ] Elimina propuesta de BD
- [ ] Cascada elimina relacionados
- [ ] Redirección a tablero automática
- [ ] Imposible eliminar en otros estados
- [ ] ID inválido retorna error

---

## Resumen de Estados Finales

Después de todas las pruebas:

| HU | Estado Final | Estudiantes | Observaciones |
|----|--------------|-------------|---------------|
| HU01 | BORRADOR | No | No |
| HU02 | BORRADOR | No | No |
| HU03 | PENDIENTE | No | No |
| HU04 | OBSERVADA → PENDIENTE | No | Sí |
| HU05 | OBSERVADA/BORRADOR | No | Posibles |
| HU06 | Cualquiera | Posibles | Posibles |
| HU07 | PENDIENTE | 3-5 | Posibles |
| HU08 | ❌ ELIMINADA | N/A | N/A |

---

## Performance y Carga

### Tiempos Esperados (< 2 segundos)

| Operación | Tiempo |
|-----------|--------|
| GET /propuestas | < 500ms |
| GET /propuestas/{id}/detalle | < 1s |
| POST /propuestas | < 500ms |
| PUT /propuestas/{id} | < 500ms |
| DELETE /propuestas/{id} | < 300ms |

### Pruebas de Carga (Opcional)

```bash
# Crear 100 propuestas
for i in {1..100}; do
  curl -X POST http://localhost:5000/api/propuestas \
    -H "Content-Type: application/json" \
    -d '{"titulo":"Test'$i'", ...}'
done
```

---

## Checklist Final

### Backend
- [ ] `dotnet build` sin errores
- [ ] `dotnet run` inicia sin excepciones
- [ ] Swagger UI accesible (`http://localhost:5000/swagger`)
- [ ] BD conectada (sin timeout)
- [ ] Logs muestran operaciones

### Frontend
- [ ] `npm start` sin errores
- [ ] Página carga en `http://localhost:4200`
- [ ] Tablero muestra propuestas
- [ ] Console sin errors críticos
- [ ] Responsive en mobile (768px)

### Datos
- [ ] >= 3 propuestas creadas
- [ ] Docentes existen
- [ ] Asignaturas existen
- [ ] Estudiantes existen
- [ ] Histórico de estados se registra

### Validaciones
- [ ] Todos los casos de prueba pasados
- [ ] Errores 400/403/404/500 retornan correctamente
- [ ] Mensajes de error son claros
- [ ] Cascadas de eliminación funcionan
- [ ] Transacciones son atómicas

---

## Reporte de Pruebas

Crear archivo `TEST_REPORT.md`:

```markdown
# Reporte de Pruebas - TesisTIC

## Fecha: 2026-05-25
## Tester: [Nombre]
## Ambiente: Development

### Resumen
- Total Casos: 25+
- Pasados: 25
- Fallidos: 0
- Tasa Éxito: 100%

### Detalles
[Copiar checklist con ✅ o ❌]

### Issues Encontrados
[Listar cualquier problema]

### Conclusión
Sistema listo para producción.
```

---
