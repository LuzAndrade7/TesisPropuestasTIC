# 📊 DOCUMENTACIÓN POSTMAN - HU02 "Visualizar tablero de propuestas TIC"

## 🎯 RESUMEN HU02

**Objetivo:** Permitir visualizar un tablero/listado de propuestas TIC con filtros por estado, estadísticas y acciones (ver/editar/eliminar).

**Endpoints involucrados:**

1. `GET /api/propuestas` - Listar propuestas con filtros opcionales
2. `GET /api/propuestas?estado=PENDIENTE` - Filtrar por estado específico
3. `GET /api/propuestas/{id}` - Ver detalle completo (desde formulario)
4. `DELETE /api/propuestas/{id}` - Eliminar propuesta

---

## 📋 CASOS DE USO

### Caso 1: Ver Tablero Completo (T05)

```
[Frontend] → [Tablero Component]
  → [PropuestaService.obtenerPropuestas()]
  → [GET /api/propuestas]
  → [Backend GetAll() - T04]
  → Devuelve: PropuestaResumenDto[] ordenado por fecha
  → Mostrar tabla con datos
```

### Caso 2: Filtrar por Estado

```
[Usuario] → [Click en estadísticas: PENDIENTE]
  → [PropuestaService.obtenerPropuestas('PENDIENTE')]
  → [GET /api/propuestas?estado=PENDIENTE]
  → Backend filtra y devuelve solo propuestas pendientes
```

### Caso 3: Calcular Estadísticas

```
Backend devuelve todas las propuestas
Frontend cuenta por cada estado (sin hacer más llamadas API)
Muestra: TODOS (total), BORRADOR, PENDIENTE, OBSERVADA, APROBADA, RECHAZADA
```

### Caso 4: Eliminar Propuesta

```
[Usuario] → [Botón Eliminar en fila]
  → [Confirma acción]
  → [PropuestaService.eliminarPropuesta(id)]
  → [DELETE /api/propuestas/{id}]
  → [Backend elimina]
  → Recarga tabla
```

---

## 🔧 ENDPOINT 1: Listar todas las propuestas

**Método:** `GET`
**URL:** `http://localhost:5000/api/propuestas`
**Ubicación Backend:** [PropuestasController.cs - GetAll()](c:\Users\USER\source\repos\TesisPropuestasTIC-1\backend\TesisTIC.API\Controllers\PropuestasController.cs#L30)

### Headers

```
(Ninguno requerido)
```

### Query Parameters

- `estado` (opcional): BORRADOR, PENDIENTE, OBSERVADA, APROBADA, RECHAZADA

### Response (200 OK)

```json
[
  {
    "id": 1,
    "nombreProyecto": "Sistema de gestión académica TIC",
    "estado": "OBSERVADA",
    "numeroParticipantes": 3,
    "fechaActualizacion": "2026-04-03T15:30:00Z",
    "fechaCreacion": "2026-03-20T10:00:00Z",
    "fechaEnvioRevision": "2026-03-25T14:00:00Z",
    "profesor": {
      "id": 1,
      "nombreCompleto": "Juan Pérez",
      "correo": "juan.perez@epn.edu.ec"
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
      }
    ]
  },
  {
    "id": 2,
    "nombreProyecto": "Aplicación móvil para tutorías estudiantiles",
    "estado": "PENDIENTE",
    "numeroParticipantes": 2,
    "fechaActualizacion": "2026-04-01T09:30:00Z",
    "fechaCreacion": "2026-03-18T14:00:00Z",
    "fechaEnvioRevision": "2026-03-28T10:00:00Z",
    "profesor": {
      "id": 2,
      "nombreCompleto": "María García",
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
]
```

### Validaciones (T04)

- ✅ Estado opcional, si se proporciona debe ser válido
- ✅ Ordena por FechaActualizacion descendente (más recientes primero)
- ✅ Devuelve PropuestaResumenDto (no información detallada)

### Estados HTTP

- **200 OK:** Lista de propuestas
- **400 Bad Request:** Estado inválido
- **500 Internal Server Error:** Error del servidor

---

## 🔧 ENDPOINT 2: Listar propuestas por estado

**Método:** `GET`
**URL:** `http://localhost:5000/api/propuestas?estado=PENDIENTE`
**Ubicación Backend:** [PropuestasController.cs - GetAll(estado)](c:\Users\USER\source\repos\TesisPropuestasTIC-1\backend\TesisTIC.API\Controllers\PropuestasController.cs#L30)

### Query Parameters

- `estado`: BORRADOR | PENDIENTE | OBSERVADA | APROBADA | RECHAZADA

### Response (200 OK)

```json
[
  {
    "id": 2,
    "nombreProyecto": "Aplicación móvil para tutorías estudiantiles",
    "estado": "PENDIENTE",
    "numeroParticipantes": 2,
    "fechaActualizacion": "2026-04-01T09:30:00Z",
    "fechaCreacion": "2026-03-18T14:00:00Z",
    "fechaEnvioRevision": "2026-03-28T10:00:00Z",
    "profesor": { ... },
    "asignaturas": [ ... ]
  }
]
```

---

## 🔧 ENDPOINT 3: Obtener detalle completo de propuesta

**Método:** `GET`
**URL:** `http://localhost:5000/api/propuestas/{id}`

### Response (200 OK)

```json
{
  "id": 1,
  "nombreProyecto": "Sistema de gestión académica TIC",
  "numeroParticipantes": 3,
  "profesorId": 1,
  "descripcion": "Un sistema integral para la gestión de propuestas TIC...",
  "objetivo": "Desarrollar una plataforma web...",
  "alcance": "El sistema será aplicable...",
  "estado": "OBSERVADA",
  "fechaCreacion": "2026-03-20T10:00:00Z",
  "fechaActualizacion": "2026-04-03T15:30:00Z",
  "fechaEnvioRevision": "2026-03-25T14:00:00Z",
  "profesor": { ... },
  "asignaturas": [ ... ]
}
```

---

## 🔧 ENDPOINT 4: Eliminar propuesta

**Método:** `DELETE`
**URL:** `http://localhost:5000/api/propuestas/{id}`

### Response (200 OK)

```json
{
  "message": "Propuesta eliminada exitosamente"
}
```

### Estados HTTP

- **200 OK:** Propuesta eliminada
- **404 Not Found:** Propuesta no existe
- **500 Internal Server Error:** Error del servidor

---

## 🚀 EJEMPLOS CON CURL

### Obtener todas las propuestas

```bash
curl "http://localhost:5000/api/propuestas"
```

### Obtener propuestas PENDIENTES

```bash
curl "http://localhost:5000/api/propuestas?estado=PENDIENTE"
```

### Obtener propuestas APROBADAS

```bash
curl "http://localhost:5000/api/propuestas?estado=APROBADA"
```

### Obtener detalle de propuesta ID=1

```bash
curl "http://localhost:5000/api/propuestas/1"
```

### Eliminar propuesta ID=1

```bash
curl -X DELETE "http://localhost:5000/api/propuestas/1"
```

---

## 📊 FLUJO COMPLETO HU02

```
1. Usuario abre tablero (http://localhost:4200)
   ↓
2. Angular navega a ruta /tablero
   ↓
3. TableroComponent.ngOnInit() se ejecuta
   ↓
4. PropuestaService.obtenerPropuestas() llamada
   ↓
5. GET /api/propuestas sin filtro
   ↓
6. Backend devuelve todas las propuestas (PropuestaResumenDto[])
   ↓
7. Frontend calcula estadísticas por estado (sin otra llamada API)
   ↓
8. Muestra tabla con:
   - Numero de fila
   - Nombre del proyecto
   - Badge de estado coloreado
   - Fecha de actualización
   - Botones: Ver, Editar, Eliminar
   ↓
9. Usuario hace click en botón "PENDIENTE" en estadísticas
   ↓
10. TableroComponent.filtrarPorEstado('PENDIENTE')
    ↓
11. Filtra array local (SIN hacer nueva llamada API)
    ↓
12. Muestra solo propuestas con estado=PENDIENTE
    ↓
13. Usuario hace click "Eliminar" en una propuesta
    ↓
14. Confirma acción
    ↓
15. DELETE /api/propuestas/{id}
    ↓
16. Backend elimina propuesta
    ↓
17. Frontend recarga: GET /api/propuestas nuevamente
```

---

## ⚡ PASOS PARA PROBAR EN POSTMAN

### Paso 1: Crear Collection

```
Nombre: HU02 - Visualizar Tablero
```

### Paso 2: Agregar Requests

#### Request 1: GET Todas las propuestas

```
Método: GET
URL: http://localhost:5000/api/propuestas
Descripción: Obtiene lista de todas las propuestas ordenadas por fecha
```

#### Request 2: GET Propuestas PENDIENTES

```
Método: GET
URL: http://localhost:5000/api/propuestas?estado=PENDIENTE
Descripción: Filtra propuestas con estado=PENDIENTE
```

#### Request 3: GET Propuestas APROBADAS

```
Método: GET
URL: http://localhost:5000/api/propuestas?estado=APROBADA
Descripción: Filtra propuestas aprobadas
```

#### Request 4: GET Propuestas OBSERVADAS

```
Método: GET
URL: http://localhost:5000/api/propuestas?estado=OBSERVADA
Descripción: Filtra propuestas con observaciones
```

#### Request 5: GET Propuestas BORRADOR

```
Método: GET
URL: http://localhost:5000/api/propuestas?estado=BORRADOR
Descripción: Filtra borradores sin enviar
```

#### Request 6: GET Propuestas RECHAZADAS

```
Método: GET
URL: http://localhost:5000/api/propuestas?estado=RECHAZADA
Descripción: Filtra propuestas rechazadas
```

#### Request 7: GET Detalle propuesta

```
Método: GET
URL: http://localhost:5000/api/propuestas/1
Descripción: Obtiene información completa de propuesta ID=1
```

#### Request 8: DELETE Propuesta

```
Método: DELETE
URL: http://localhost:5000/api/propuestas/1
Descripción: Elimina propuesta ID=1
```

### Paso 3: Ejecutar Requests

1. Ejecutar Request 1 → Ver todas las propuestas
2. Ejecutar Request 2 → Ver solo PENDIENTES
3. Ejecutar Request 7 → Ver detalle de una propuesta
4. Ejecutar Request 8 → Eliminar una propuesta
5. Ejecutar Request 1 → Confirmar que fue eliminada

---

## 📊 ESTADÍSTICAS EN TABLERO

El frontend calcula automáticamente (sin hacer llamadas API adicionales):

```javascript
estadisticas = {
  total: propuestas.length,
  borrador: propuestas.filter((p) => p.estado === "BORRADOR").length,
  pendiente: propuestas.filter((p) => p.estado === "PENDIENTE").length,
  observada: propuestas.filter((p) => p.estado === "OBSERVADA").length,
  aprobada: propuestas.filter((p) => p.estado === "APROBADA").length,
  rechazada: propuestas.filter((p) => p.estado === "RECHAZADA").length,
};
```

Estos números aparecen en los botones de estadísticas del tablero.

---

## 🎨 INTERFAZ USUARIO (T05)

### Elementos del Tablero:

1. **Encabezado**: "Mis propuestas" con subtítulo
2. **Botón "Nueva propuesta"**: Navega a /propuestas/nueva
3. **Estadísticas**: 6 botones (TODOS + 5 estados)
4. **Tabla**: Con columnas (N°, Título, Estado, Actualización, Acciones)
5. **Badges coloreados**: Por estado con puntos de color
6. **Botones de acción**: Ver, Editar, Eliminar
7. **Mensaje vacío**: Cuando no hay propuestas

---

## 🔐 DIFERENCIAS CON HU01

| Aspecto        | HU01                         | HU02                  |
| -------------- | ---------------------------- | --------------------- |
| **Objetivo**   | Crear propuesta              | Visualizar propuestas |
| **Endpoint**   | POST /api/propuestas         | GET /api/propuestas   |
| **Filtros**    | Ninguno                      | Por estado (opcional) |
| **DTO**        | CreatePropuestaDto           | PropuestaResumenDto   |
| **Acciones**   | Guardar, Enviar              | Ver, Editar, Eliminar |
| **Componente** | FormularioPropuestaComponent | TableroComponent      |

---

## 🔗 REFERENCIAS RÁPIDAS

- **Backend Endpoint:** [PropuestasController.GetAll()](c:\Users\USER\source\repos\TesisPropuestasTIC-1\backend\TesisTIC.API\Controllers\PropuestasController.cs)
- **DTO Resumen:** [PropuestaResumenDto.cs](c:\Users\USER\source\repos\TesisPropuestasTIC-1\backend\TesisTIC.Application\DTOs\PropuestaResumenDto.cs)
- **Frontend Service:** [propuesta.service.ts](c:\Users\USER\source\repos\TesisPropuestasTIC-1\frontend\src\app\services\propuesta.service.ts)
- **Frontend Component:** [tablero.component.ts](c:\Users\USER\source\repos\TesisPropuestasTIC-1\frontend\src\app\components\tablero\tablero.component.ts)

---

## 📱 COMPATIBLE CON:

- ✅ Postman (Desktop/Web)
- ✅ Insomnia
- ✅ Thunder Client
- ✅ curl desde terminal
- ✅ Frontend Angular con RxJS Observables
