# 🎯 HU01 - REGISTRAR Y GUARDAR PROPUESTA TIC

## ✅ ESTADO: COMPLETADO 100%

Se ha implementado completamente la Historia de Usuario HU01 con:

- ✅ T01 Backend
- ✅ T02 Frontend
- ✅ T03 Integración
- ✅ Documentación

---

## 📁 ARCHIVOS CREADOS

### BACKEND (.NET 8)

**Ubicación:** `backend/`

No se requirió crear nuevos archivos backend porque ya existían:

- ✅ `TesisTIC.API/Controllers/PropuestasController.cs` - Endpoint POST crear propuesta
- ✅ `TesisTIC.Application/Services/PropuestaService.cs` - Lógica guardar borrador
- ✅ `TesisTIC.Application/DTOs/PropuestaDto.cs` - DTO con validaciones
- ✅ `TesisTIC.Infrastructure/Repositories/PropuestaRepository.cs` - Acceso a datos

**Validaciones implementadas (T01):**

- ✅ Nombre proyecto: 10-250 caracteres
- ✅ Participantes: máximo 5 (T01 específico)
- ✅ Estado por defecto: BORRADOR
- ✅ Permitir guardar incompleto
- ✅ Async/Await en todo

---

### FRONTEND ANGULAR

**Ubicación:** `frontend/src/`

#### 1. Configuración Principal

- ✅ `src/environments/environment.ts` - URL API (localhost:5000)
- ✅ `src/environments/environment.prod.ts` - URLs producción
- ✅ `src/main.ts` - Bootstrap angular
- ✅ `src/app/app.config.ts` - Configuración app (HttpClient, Router)
- ✅ `src/app/app.routes.ts` - Rutas
- ✅ `src/app/app.component.ts/html/scss` - Componente raíz
- ✅ `src/index.html` - HTML raíz
- ✅ `src/styles.scss` - Estilos globales
- ✅ `angular.json` - Configuración Angular CLI
- ✅ `package.json` - Dependencias
- ✅ `tsconfig.json` - Configuración TypeScript

#### 2. Modelos y Servicios (T03)

- ✅ `src/app/models/propuesta.model.ts` - Interfaces TypeScript
  - `Propuesta`
  - `CreatePropuestaRequest`
  - `UpdatePropuestaRequest`
  - `CambiarEstadoRequest`
  - `Asignatura`
  - `Docente`

- ✅ `src/app/services/propuesta.service.ts` - Servicio HTTP
  - `crearPropuesta()` - POST /api/propuestas (T01)
  - `guardarBorrador()` - POST /api/propuestas (T01)
  - `cambiarEstado()` - PATCH /api/propuestas/{id}/estado (T03)
  - `obtenerAsignaturas()` - GET /api/asignaturas (T02)
  - `obtenerDocentes()` - GET /api/docentes (T02)
  - Manejo centralizado de errores

#### 3. Componente Formulario (T02)

- ✅ `src/app/components/formulario-propuesta/formulario-propuesta.component.ts`
  - FormGroup reactivo con validaciones
  - Métodos:
    - `guardarBorrador()` - Guardar como borrador (T01)
    - `enviarPropuesta()` - Enviar a revisión (T03)
    - `cargarAsignaturas()` - Cargar dropdown
    - `cargarDocentes()` - Cargar dropdown
    - `cargarPropuesta()` - Para edición
  - Manejo de errores y mensajes
  - Estados: cargando, guardando, enviando

- ✅ `src/app/components/formulario-propuesta/formulario-propuesta.component.html`
  - Formulario profesional responsivo
  - Secciones:
    1. Información Básica (Nombre, Participantes, Profesor)
    2. Descripción Detallada (Descripción, Objetivo, Alcance)
    3. Asignaturas (Checkboxes múltiples)
  - Validaciones en tiempo real
  - Contadores de caracteres
  - Botones: Guardar Borrador, Enviar, Cancelar
  - Mensajes de error y éxito

- ✅ `src/app/components/formulario-propuesta/formulario-propuesta.component.scss`
  - Diseño profesional responsivo
  - Estilos para formularios, campos, botones
  - Validaciones visuales
  - Animaciones
  - Mobile responsive

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
  apiBaseUrl: "http://localhost:5000",
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
```

---

## 📊 ENDPOINTS HU01

### T01: Guardar como Borrador

```
POST /api/propuestas
Content-Type: application/json

Body:
{
  "nombreProyecto": "Mi Propuesta",
  "numeroParticipantes": 2,
  "profesorId": 1,
  "descripcion": "Descripción...",
  "objetivo": "Objetivo...",
  "alcance": "Alcance...",
  "asignaturaIds": [1, 2]
}

Response (201):
{
  "id": 1,
  "estado": "BORRADOR",
  "fechaCreacion": "2026-05-17T...",
  ...
}
```

### T03: Enviar a Revisión

```
PATCH /api/propuestas/1/estado
Content-Type: application/json

Body:
{
  "estado": "PENDIENTE"
}

Response (200):
{
  "id": 1,
  "estado": "PENDIENTE",
  "fechaEnvioRevision": "2026-05-17T...",
  ...
}
```

---

## 🎯 VALIDACIONES IMPLEMENTADAS (T01)

✅ **Frontend (Angular Reactive Forms)**

- Campo requerido: todas las propiedades
- Nombre: 10-250 caracteres con contador
- Participantes: 1-5 máximo (T01)
- Descripción: 20-2000 caracteres
- Objetivo: 20-2000 caracteres
- Alcance: 20-2000 caracteres
- Asignaturas: mínimo 1

✅ **Backend (.NET)**

- Validaciones en PropuestaService
- Estados válidos: BORRADOR, PENDIENTE, OBSERVADA, APROBADA, RECHAZADA
- Guardar con estado BORRADOR por defecto
- Permitir guardar incompleto (solo frontend valida)

---

## 📱 FUNCIONALIDADES HU01

### ✅ T01: Guardar como Borrador

1. Usuario completa formulario (parcial o completo)
2. Click "Guardar como Borrador"
3. Frontend valida campos requeridos
4. POST /api/propuestas
5. Backend crea propuesta con `estado: BORRADOR`
6. Usuario ve mensaje de éxito
7. Propuesta guardada con ID

### ✅ T02: Interfaz Formulario

1. Formulario reactivo profesional
2. Dropdown dinámico para docentes
3. Checkboxes múltiples para asignaturas
4. Validaciones en tiempo real
5. Mensajes de error por campo
6. Contadores de caracteres
7. Estados: cargando, guardando, enviando
8. Responsivo (mobile, tablet, desktop)

### ✅ T03: Enviar a Revisión

1. Usuario hace click "Enviar a Revisión"
2. Propuesta se guarda primero si es nueva
3. PATCH /api/propuestas/{id}/estado {estado: PENDIENTE}
4. Backend cambia estado a PENDIENTE
5. Backend registra fechaEnvioRevision
6. Usuario ve mensaje de éxito
7. Redirige a página de propuesta

---

## 📊 STACK TECNOLÓGICO

**Backend:**

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL Neon
- C# 11

**Frontend:**

- Angular 17
- TypeScript 5.2
- RxJS 7.8
- Angular Reactive Forms
- SCSS

**Base de Datos:**

- PostgreSQL Neon (Cloud)
- Tablas: propuestas, propuesta_asignaturas, docentes, asignaturas

---

## 🔍 PRUEBAS

### Prueba 1: Guardar Borrador

1. Ejecutar backend: `dotnet run`
2. Ejecutar frontend: `npm start`
3. Abrir http://localhost:4200
4. Llenar formulario
5. Click "Guardar como Borrador"
6. ✅ Debe mostrar "Propuesta guardada como borrador"

### Prueba 2: Enviar a Revisión

1. Continuar desde Prueba 1
2. Click "Enviar a Revisión"
3. ✅ Debe mostrar "Propuesta enviada a revisión"
4. Verificar en Postman: `GET /api/propuestas/1` → estado = "PENDIENTE"

### Prueba 3: Postman

1. Importar endpoints de [HU01_POSTMAN.md](HU01_POSTMAN.md)
2. GET /api/docentes ✅
3. GET /api/asignaturas ✅
4. POST /api/propuestas ✅
5. PATCH /api/propuestas/1/estado ✅

---

## 📋 CHECKLIST HU01

### Tareas Completadas

- [x] T01: Implementar backend crear/guardar propuesta
  - [x] POST /api/propuestas endpoint
  - [x] Validaciones (max 5 participantes)
  - [x] Estado BORRADOR por defecto
  - [x] Async/await
  - [x] DTOs y validaciones

- [x] T02: Diseñar interfaz formulario
  - [x] Formulario reactivo
  - [x] Validaciones tiempo real
  - [x] Dropdowns dinámicos
  - [x] Checkboxes asignaturas
  - [x] Diseño responsivo
  - [x] Mensajes error/éxito

- [x] T03: Conectar frontend-backend
  - [x] PropuestaService HTTP
  - [x] Environment.ts configurado
  - [x] Manejo errores
  - [x] Integración visual
  - [x] Botones funcionales
  - [x] Cambio estado PENDIENTE

- [x] Documentación
  - [x] Comentarios código (T01, T02, T03)
  - [x] HU01_POSTMAN.md
  - [x] README.md HU01

---

## 📚 ARCHIVOS CLAVE

**Backend:**

```
backend/
├── TesisTIC.API/Controllers/PropuestasController.cs
├── TesisTIC.Application/Services/PropuestaService.cs
├── TesisTIC.Application/DTOs/PropuestaDto.cs
└── TesisTIC.Infrastructure/Repositories/PropuestaRepository.cs
```

**Frontend:**

```
frontend/
├── src/
│   ├── app/
│   │   ├── components/formulario-propuesta/
│   │   │   ├── formulario-propuesta.component.ts (T02)
│   │   │   ├── formulario-propuesta.component.html (T02)
│   │   │   └── formulario-propuesta.component.scss (T02)
│   │   ├── services/
│   │   │   └── propuesta.service.ts (T03)
│   │   ├── models/
│   │   │   └── propuesta.model.ts (T03)
│   │   ├── app.routes.ts
│   │   ├── app.config.ts
│   │   └── app.component.ts
│   ├── environments/
│   │   ├── environment.ts (T03)
│   │   └── environment.prod.ts
│   ├── main.ts
│   └── styles.scss
├── angular.json
├── package.json
└── tsconfig.json
```

---

## 🔗 DOCUMENTACIÓN

- **[HU01_POSTMAN.md](HU01_POSTMAN.md)** - Endpoints, ejemplos curl, pruebas
- **[ESTRUCTURA_BACKEND.md](ESTRUCTURA_BACKEND.md)** - Arquitectura backend
- **[ENDPOINTS_POSTMAN.md](ENDPOINTS_POSTMAN.md)** - Todos los endpoints
- **[GUIA_EJECUCION.md](GUIA_EJECUCION.md)** - Cómo ejecutar backend
- **[README.md](README.md)** - Resumen proyecto

---

## ⚡ PRÓXIMAS FASES

Después de HU01, implementar:

### HU02: Editar Propuesta

- PUT /api/propuestas/{id}
- Componente edición
- Cargar datos existentes

### HU03: Ver Propuestas

- GET /api/propuestas (listado)
- GET /api/propuestas/{id} (detalle)
- Tabla/Grid propuestas
- Filtros estado

### HU04: Observaciones CPGIC

- POST /api/observaciones
- Formulario observaciones
- Listar observaciones

### HU05: Aprobación CPGIC

- PATCH /api/propuestas/{id}/aprobacion
- Firmas digitales
- Resolución

---

## 📞 SOPORTE

Si tienes dudas sobre HU01:

1. **Backend:** Revisa [PropuestasController.cs](backend/TesisTIC.API/Controllers/PropuestasController.cs)
2. **Frontend:** Revisa [formulario-propuesta.component.ts](frontend/src/app/components/formulario-propuesta/formulario-propuesta.component.ts)
3. **Endpoints:** Consulta [HU01_POSTMAN.md](HU01_POSTMAN.md)
4. **Ejecución:** Lee [GUIA_EJECUCION.md](GUIA_EJECUCION.md)

---

**HU01 COMPLETADA ✅**
Fecha: 2026-05-17
Versión: 1.0.0
