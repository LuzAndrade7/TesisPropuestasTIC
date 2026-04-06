# TesisTIC - Módulo Propuestas de TIC

Sistema web para la gestión de propuestas de Trabajos de Integración Curricular en la Facultad de Ingeniería de Sistemas.

## Descripción del Proyecto

**Módulo Específico:** Propuestas de TIC

Este módulo permite:

- Registrar propuestas de proyectos de titulación by docentes
- Enviar propuestas para revisión por la CPGIC
- Consultar el estado de la propuesta (Pendiente, Aprobada, Rechazada)
- Editar propuestas en caso de observaciones
- Asignar estudiantes a propuestas aprobadas

## Arquitectura

El proyecto sigue una arquitectura en capas:

```
Frontend (Angular 17)
    ↓
Backend API (.NET 8)
    ↓
Servicios y Repositorios
    ↓
Base de Datos (PostgreSQL 16)
```

### Estructura de Capas Backend

- **TesisTIC.API**: Capa de presentación (Controllers, modelos API)
- **TesisTIC.Application**: Lógica de aplicación (Services, DTOs, Interfaces)
- **TesisTIC.Domain**: Modelos de dominio (Entities, Value Objects)
- **TesisTIC.Infrastructure**: Acceso a datos (Repositories, DbContext, EF Core)

## Tecnologías

### Backend

- **.NET 8** - Framework principal
- **ASP.NET Core Web API** - API REST
- **Entity Framework Core 8** - ORM
- **PostgreSQL 16** - Base de datos
- **Npgsql** - Driver PostgreSQL para EF Core

### Frontend

- **Angular 17** - Framework principal
- **Bootstrap 5** - Estilos y componentes UI
- **Reactive Forms** - Manejo de formularios
- **RxJS** - Manejo de observables

### DevOps & Tools

- **Git** - Control de versiones
- **Docker** (opcional)
- **Visual Studio Code** - IDE recomendado

## Instalación

### Requisitos Previos

- .NET 8 SDK
- Node.js 18+ y npm
- PostgreSQL 16
- Git

### 1. Configurar Base de Datos

```bash
# Conectarse a PostgreSQL
psql -U postgres

# Ejecutar el script de inicialización
\i database/init.sql
```

### 2. Configurar Backend

```bash
# Navegar al directorio del backend
cd backend

# Restaurar dependencias
dotnet restore

# Compilar el proyecto
dotnet build

# Ejecutar migraciones (si aplica)
dotnet ef database update --project TesisTIC.Infrastructure --startup-project TesisTIC.API

# Ejecutar la API
dotnet run --project TesisTIC.API/TesisTIC.API.csproj
```

La API estará disponible en `http://localhost:5000`

### 3. Configurar Frontend

```bash
# Navegar al directorio del frontend
cd frontend/tesis-tic-app

# Instalar dependencias
npm install

# Ejecutar servidor de desarrollo
npm start
```

La aplicación estará disponible en `http://localhost:4200`

## API Endpoints

### Propuestas

```
POST   /api/propuestas                    - Crear propuesta
GET    /api/propuestas                    - Obtener todas
GET    /api/propuestas/{id}               - Obtener por ID
GET    /api/propuestas/docente/{docenteId} - Obtener por docente
GET    /api/propuestas/estado/{estadoId}   - Obtener por estado
PUT    /api/propuestas/{id}               - Actualizar propuesta
PATCH  /api/propuestas/{id}/estado        - Cambiar estado
POST   /api/propuestas/{id}/estudiantes   - Asignar estudiante
DELETE /api/propuestas/{id}               - Eliminar propuesta
```

### Estados

```
GET    /api/estados                       - Listar todos los estados
```

### Docentes

```
GET    /api/docentes                      - Listar todos
GET    /api/docentes/{id}                 - Obtener por ID
```

### Asignaturas

```
GET    /api/asignaturas                   - Listar todas
```

## Estructura de Carpetas

```
TesisPropuestasTIC-1/
├── backend/
│   ├── TesisTIC.API/
│   │   ├── Controllers/
│   │   ├── Properties/
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── TesisTIC.API.csproj
│   ├── TesisTIC.Application/
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   └── TesisTIC.Application.csproj
│   ├── TesisTIC.Domain/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   └── TesisTIC.Domain.csproj
│   ├── TesisTIC.Infrastructure/
│   │   ├── Persistence/
│   │   ├── Repositories/
│   │   └── TesisTIC.Infrastructure.csproj
│   └── TesisTIC.sln
├── database/
│   ├── init.sql
│   └── migrations/
├── frontend/
│   └── tesis-tic-app/
│       ├── src/
│       │   ├── app/
│       │   │   ├── modules/propuestas/
│       │   │   ├── shared/models/
│       │   │   └── app.component.ts
│       │   ├── environments/
│       │   └── main.ts
│       ├── angular.json
│       ├── package.json
│       └── tsconfig.json
├── docs/
└── README.md
```

## Flujo de Trabajo - Git

1. Crear rama de feature

   ```bash
   git checkout -b feature/nueva-funcionalidad
   ```

2. Realizar cambios y commits

   ```bash
   git add .
   git commit -m "feat: descripción del cambio"
   ```

3. Push a rama

   ```bash
   git push origin feature/nueva-funcionalidad
   ```

4. Crear Pull Request

5. Merge a main después de revisión

## Ejemplo de Uso

### Crear una Propuesta

**Request:**

```bash
POST /api/propuestas
Content-Type: application/json

{
  "titulo": "Sistema de Gestión de Proyectos",
  "descripcion": "Desarrollo de una aplicación web para gestionar proyectos de software.",
  "objetivo": "Crear una aplicación robusta y escalable",
  "alcance": "Módulo de gestión y seguimiento",
  "componentesActividadesProductos": "API REST, Frontend Angular, BD PostgreSQL",
  "docenteId": 1,
  "lineaInvestigacionId": 1,
  "numeroParticipantes": 2,
  "departamento": "Ingeniería de Software",
  "facultad": "Facultad de Ingeniería de Sistemas",
  "asignaturasIds": [1, 2]
}
```

**Response:**

```json
{
  "id": 1,
  "titulo": "Sistema de Gestión de Proyectos",
  "estadoNombre": "Pendiente",
  "fechaCreacion": "2024-01-15T10:30:00Z"
}
```

## Testing

### Backend

```bash
# Ejecutar pruebas unitarias
dotnet test
```

### Frontend

```bash
# Ejecutar pruebas
npm test

# Ejecutar pruebas con cobertura
npm run test:coverage
```

## Buenas Prácticas

- **Código Limpio:** Nombres claros, funciones pequeñas, sin duplicación
- **SOLID Principles:** Especialmente SRP e ISP
- **Separation of Concerns:** Cada componente tiene una responsabilidad
- **DRY:** Don't Repeat Yourself
- **Naming Conventions:** camelCase para variables/funciones, PascalCase para clases
- **Error Handling:** Manejo consistente de errores
- **Logging:** Registro de eventos importantes

## Autores

Proyecto de Tesis - Ingeniería de Software
Módulo: Propuestas de TIC

## Licencia

Este proyecto es propiedad de la Facultad de Ingeniería de Sistemas.

## Notas de Desarrollo

### Migraciones Entity Framework (si se necesitan en futuro)

```bash
# Crear nueva migración
dotnet ef migrations add NombreMigracion --project TesisTIC.Infrastructure --startup-project TesisTIC.API

# Aplicar migraciones
dotnet ef database update --project TesisTIC.Infrastructure --startup-project TesisTIC.API
```

### Puerto de Desarrollo

- Backend: http://localhost:5000
- Frontend: http://localhost:4200
- PostgreSQL: localhost:5432

### Credenciales Por Defecto (Desarrollo)

```
Base de Datos: tesis_tic_db
Usuario: postgres
Contraseña: postgres
```

⚠️ **NOTA:** Cambiar estas credenciales en producción

---

Para más información o preguntas sobre la implementación, revisar la documentación en la carpeta `/docs`
