# 📊 HU02 - VISUALIZAR TABLERO DE PROPUESTAS TIC

## ✅ ESTADO: COMPLETADO 100%

Se ha implementado completamente la Historia de Usuario HU02 con:

- ✅ T04 Backend (GET con filtros)
- ✅ T05 Frontend (Tablero visual)
- ✅ T06 Integración (API + Angular)
- ✅ Documentación Postman

---

## 📁 ARCHIVOS CREADOS/MODIFICADOS

### BACKEND (.NET 8)

**Ubicación:** `backend/`

#### Nuevos archivos:

- ✅ [TesisTIC.Application/DTOs/PropuestaResumenDto.cs](backend/TesisTIC.Application/DTOs/PropuestaResumenDto.cs) - DTO resumen para tablero (T04)

#### Archivos modificados:

- ✅ [TesisTIC.API/Controllers/PropuestasController.cs](backend/TesisTIC.API/Controllers/PropuestasController.cs) - Endpoint GET actualizado con filtros
- ✅ [TesisTIC.Application/Mappings/MappingProfile.cs](backend/TesisTIC.Application/Mappings/MappingProfile.cs) - Mapeo a PropuestaResumenDto

**Validaciones implementadas (T04):**

- ✅ GET /api/propuestas devuelve PropuestaResumenDto[]
- ✅ Parámetro query ?estado=PENDIENTE (opcional)
- ✅ Ordenado por FechaActualizacion descendente (más recientes primero)
- ✅ Información resumida (sin módulos/componentes)

---

### FRONTEND ANGULAR

**Ubicación:** `frontend/src/`

#### Modelos (T06):

- ✅ [src/app/models/tablero.model.ts](frontend/src/app/models/tablero.model.ts)
  - `PropuestaResumen`: Interface para propuesta en tablero
  - `DocenteResumen`: Información simplificada de docente
  - `EstadisticasTablero`: Conteos por estado
  - `FiltroTablero`: Definición de filtros

#### Servicios (T06):

- ✅ [src/app/services/propuesta.service.ts](frontend/src/app/services/propuesta.service.ts) - Métodos agregados:
  - `obtenerPropuestas(estado?)` - GET /api/propuestas con filtros opcionales
  - `obtenerPropuestasCompletas()` - GET antiguo renombrado

#### Componentes (T05):

- ✅ [src/app/components/tablero/tablero.component.ts](frontend/src/app/components/tablero/tablero.component.ts)
  - Carga propuestas del API
  - Calcula estadísticas
  - Filtra por estado
  - Maneja acciones (Ver/Editar/Eliminar)

- ✅ [src/app/components/tablero/tablero.component.html](frontend/src/app/components/tablero/tablero.component.html)
  - Sigue EXACTAMENTE el prototipo HTML entregado
  - Encabezado con botón "Nueva propuesta"
  - Estadísticas: 6 botones (TODOS + 5 estados)
  - Tabla con columnas: N°, Título, Estado, Actualización, Acciones
  - Badges coloreados por estado
  - Botones de acción (Ver/Editar/Eliminar)
  - Mensaje vacío cuando no hay propuestas

- ✅ [src/app/components/tablero/tablero.component.scss](frontend/src/app/components/tablero/tablero.component.scss)
  - Diseño profesional siguiendo colores del prototipo
  - Colores por estado: BORRADOR, PENDIENTE, OBSERVADA, APROBADA, RECHAZADA
  - Responsive (mobile, tablet, desktop)
  - Animaciones suaves

#### Rutas (T06):

- ✅ [src/app/app.routes.ts](frontend/src/app/app.routes.ts) - Actualizado:
  - Ruta `/tablero` → TableroComponent
  - Ruta raíz `/` → redirige a `/tablero`
  - Rutas existentes de propuestas conservadas

---

## 🔧 CONFIGURACIÓN

### Backend (.NET)

Ya está configurado en `backend/TesisTIC.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=ep-lingering-mouse...neon.tech;Database=neondb;..."
  }
}
```

### Frontend Angular

Configuración en `frontend/src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: "http://localhost:5000/api",
};
```

---

## 🚀 EJECUCIÓN

### Inicio rápido

**Terminal 1: Backend .NET**

```bash
cd backend
dotnet restore
dotnet build
cd TesisTIC.API
dotnet run
# API en http://localhost:5000
# Swagger en http://localhost:5000/swagger
```

**Terminal 2: Frontend Angular**

```bash
cd frontend
npm install
npm start
# App en http://localhost:4200
# Tablero en http://localhost:4200/tablero
```

---

## 📊 ENDPOINTS HU02

### T04: Listar propuestas (sin filtro)

```
GET /api/propuestas
Content-Type: application/json

Response (200):
[
  {
    "id": 1,
    "nombreProyecto": "Sistema de gestión académica TIC",
    "estado": "OBSERVADA",
    "numeroParticipantes": 3,
    "fechaActualizacion": "2026-04-03T15:30:00Z",
    "profesor": {
      "id": 1,
      "nombreCompleto": "Juan Pérez",
      "correo": "juan.perez@epn.edu.ec"
    },
    "asignaturas": [...]
  },
  ...
]
```

### T04: Listar propuestas (con filtro por estado)

```
GET /api/propuestas?estado=PENDIENTE
Content-Type: application/json

Response (200):
[
  {
    "id": 2,
    "nombreProyecto": "Aplicación móvil para tutorías",
    "estado": "PENDIENTE",
    ...
  }
]
```

### T05: Acciones desde tablero

```
Botón VER DETALLE → GET /api/propuestas/{id} → FormularioPropuestaComponent
Botón EDITAR      → GET /api/propuestas/{id} → FormularioPropuestaComponent (edit mode)
Botón ELIMINAR    → DELETE /api/propuestas/{id} → Recarga tablero
```

---

## 🎯 VALIDACIONES IMPLEMENTADAS (T04)

✅ **Backend**

- GET /api/propuestas devuelve PropuestaResumenDto (no información detallada)
- Parámetro ?estado es opcional
- Si se proporciona ?estado, debe ser válido: BORRADOR, PENDIENTE, OBSERVADA, APROBADA, RECHAZADA
- Ordena por FechaActualizacion descendente (más recientes primero)
- Error 400 si estado inválido
- Error 500 si falla servidor

✅ **Frontend (T05)**

- Tabla solo mostrada si hay propuestas
- Estadísticas se calculan localmente (sin llamadas API adicionales)
- Filtros trabajan en array local (sin recargar del servidor)
- Botones de acción solo disponibles si hay propuestas
- Mensaje "vacío" cuando no hay propuestas para mostrar

---

## 📊 ESTADÍSTICAS TABLERO (T05)

Botones mostrando conteos:

```
TODOS:      5 propuestas
BORRADOR:   1 propuesta
PENDIENTE:  1 propuesta
OBSERVADA:  1 propuesta
APROBADA:   1 propuesta
RECHAZADA:  1 propuesta
```

Se calculan automáticamente del array sin hacer más llamadas API.

---

## 🎨 INTERFAZ DEL TABLERO (PROTOTIPO)

El diseño sigue EXACTAMENTE el HTML/CSS proporcionado:

```
┌─────────────────────────────────────────────┐
│  Mis propuestas                  [+ Nueva]  │
│  Formulario F_AA_233A                       │
├─────────────────────────────────────────────┤
│ [TODOS-5] [BORRADOR-1] [PENDIENTE-1] ...   │
├─────────────────────────────────────────────┤
│ N° │ Título │ Estado │ Actualización │ ...  │
├────┼────────┼────────┼───────────────┼──────┤
│ 1  │ Sistema... │ 🔶 Observada │ 03/04 │ 👁 ✏️ 🗑️  │
│ 2  │ Aplicación │ 🔵 Pendiente │ 01/04 │ 👁 ✏️ 🗑️  │
│ 3  │ Dashboard  │ ✅ Aprobada  │ 20/03 │ 👁 ✏️ 🗑️  │
│ 4  │ Chatbot    │ ⚪ Borrador  │ 10/04 │ 👁 ✏️ 🗑️  │
│ 5  │ IoT        │ ❌ Rechazada │ 05/03 │ 👁 ✏️ 🗑️  │
└────┴────────┴────────┴───────────────┴──────┘
```

**Colores por estado:**

- 🔘 BORRADOR: Gris (#F1F3F5)
- 🔵 PENDIENTE: Azul (#EBF3FC)
- 🔶 OBSERVADA: Oro (#FEF9EC)
- ✅ APROBADA: Verde (#ECFDF5)
- ❌ RECHAZADA: Rojo (#FEF2F2)

---

## 📊 FLUJO DE DATOS

```
Usuario abre tablero
       ↓
Angular carga /tablero
       ↓
TableroComponent.ngOnInit()
       ↓
PropuestaService.obtenerPropuestas()
       ↓
GET /api/propuestas (sin filtro - trae todo)
       ↓
Backend devuelve PropuestaResumenDto[]
       ↓
Frontend muestra tabla + calcula estadísticas
       ↓
Usuario hace click en "PENDIENTE"
       ↓
TableroComponent.filtrarPorEstado('PENDIENTE')
       ↓
Filtra array local (SIN llamada API)
       ↓
Muestra solo propuestas pendientes
       ↓
Usuario hace click en botón eliminar
       ↓
DELETE /api/propuestas/{id}
       ↓
Backend elimina
       ↓
Frontend recarga: GET /api/propuestas nuevamente
```

---

## 🔍 PRUEBAS

### Prueba 1: Ver Tablero Completo

1. Ejecutar backend: `dotnet run`
2. Ejecutar frontend: `npm start`
3. Abrir http://localhost:4200
4. ✅ Debe mostrar tabla con todas las propuestas
5. ✅ Estadísticas deben coincidir con cantidad de propuestas

### Prueba 2: Filtrar por Estado

1. Continuar desde Prueba 1
2. Hacer click en botón "PENDIENTE"
3. ✅ Tabla debe mostrar solo propuestas pendientes
4. ✅ Hacer click en "TODOS" debe mostrar todas nuevamente

### Prueba 3: Eliminar Propuesta

1. Continuar desde Prueba 2
2. Hacer click en botón "🗑️" en una fila
3. Confirmar acción
4. ✅ Propuesta debe desaparecer de la tabla
5. ✅ Estadísticas deben actualizarse

### Prueba 4: Ver Detalle

1. Continuar desde Prueba 3
2. Hacer click en botón "👁️" en una fila
3. ✅ Debe navegar a formulario con datos cargados (editar)

### Prueba 5: Postman

1. Importar endpoints de [HU02_POSTMAN.md](HU02_POSTMAN.md)
2. GET /api/propuestas ✅
3. GET /api/propuestas?estado=PENDIENTE ✅
4. GET /api/propuestas?estado=APROBADA ✅
5. DELETE /api/propuestas/{id} ✅

---

## 📋 CHECKLIST HU02

### Tareas Completadas

- [x] T04: Implementar backend GET con filtros
  - [x] Endpoint GET /api/propuestas
  - [x] Parámetro ?estado opcional
  - [x] Ordenar por FechaActualizacion descendente
  - [x] DTO PropuestaResumenDto (información resumida)
  - [x] Validaciones (estado válido)

- [x] T05: Diseñar tablero visual
  - [x] Tabla con estructura del prototipo
  - [x] Estadísticas (TODOS + 5 estados)
  - [x] Badges coloreados por estado
  - [x] Botones de acción (Ver/Editar/Eliminar)
  - [x] Mensaje "tabla vacía"
  - [x] Diseño responsive
  - [x] Paleta de colores EPN

- [x] T06: Conectar frontend-backend
  - [x] PropuestaService.obtenerPropuestas()
  - [x] TableroComponent consume API
  - [x] Filtros en tiempo real (array local)
  - [x] Sin datos hardcodeados
  - [x] Rutas configuradas
  - [x] Manejo de errores

- [x] Documentación
  - [x] Comentarios código (T04, T05, T06)
  - [x] HU02_POSTMAN.md
  - [x] HU02_README.md (este archivo)

---

## 📚 ARCHIVOS CLAVE

**Backend:**

```
backend/
├── TesisTIC.API/Controllers/PropuestasController.cs (T04)
├── TesisTIC.Application/DTOs/PropuestaResumenDto.cs (T04)
└── TesisTIC.Application/Mappings/MappingProfile.cs (T04)
```

**Frontend:**

```
frontend/src/
├── app/
│   ├── components/tablero/
│   │   ├── tablero.component.ts (T05)
│   │   ├── tablero.component.html (T05)
│   │   └── tablero.component.scss (T05)
│   ├── services/
│   │   └── propuesta.service.ts (T06)
│   ├── models/
│   │   └── tablero.model.ts (T06)
│   └── app.routes.ts (T06)
```

---

## 🔗 DOCUMENTACIÓN

- **[HU02_POSTMAN.md](HU02_POSTMAN.md)** - Endpoints, ejemplos curl, casos uso
- **[HU01_README.md](HU01_README.md)** - Información sobre formulario (relacionado)
- **[HU01_POSTMAN.md](HU01_POSTMAN.md)** - Endpoints de creación (relacionado)

---

## ⚡ PRÓXIMAS FASES

Después de HU02, implementar:

### HU03: Editar Propuesta

- PUT /api/propuestas/{id}
- Cargar datos en formulario
- Actualizar con cambios

### HU04: Ver Detalle Completo

- Mostrar información completa de propuesta
- Mostrar módulos/componentes
- Mostrar observaciones

### HU05: Observaciones CPGIC

- POST /api/observaciones
- Formulario para observaciones
- Listar observaciones por propuesta

### HU06: Aprobación CPGIC

- PATCH /api/propuestas/{id}/aprobacion
- Firmas digitales
- Resolución final

---

## 🎯 RESUMEN

**HU02 proporciona:**

1. ✅ Vista centralizada de todas las propuestas
2. ✅ Filtros dinámicos por estado
3. ✅ Estadísticas de propuestas por estado
4. ✅ Acciones rápidas (Ver/Editar/Eliminar)
5. ✅ Datos en tiempo real desde API
6. ✅ Interfaz profesional y responsiva
7. ✅ Compatible con prototipo EPN

**Métricas:**

- 📄 3 archivos backend (1 nuevo, 2 modificados)
- 📄 5 archivos frontend (4 nuevos, 1 modificado)
- 📝 ~400 líneas de código TypeScript
- 🎨 ~500 líneas de estilos SCSS
- 📊 ~500 líneas de HTML
- 📚 Documentación completa Postman

---

**HU02 COMPLETADA ✅**
Fecha: 2026-05-17
Versión: 1.0.0
