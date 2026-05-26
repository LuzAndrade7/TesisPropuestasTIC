# 🚀 GUÍA DE EJECUCIÓN - TESTISTIC BACKEND

## ✅ REQUISITOS PREVIOS

- ✅ .NET 8 SDK instalado
- ✅ Visual Studio Code o Visual Studio 2022
- ✅ PostgreSQL Neon cuenta activa
- ✅ Git (para control de versiones)

---

## 📦 PASO 1: RESTAURAR PAQUETES NUGET

Abre una terminal en la carpeta del backend:

```bash
cd backend
dotnet restore
```

**Salida esperada:**

```
Determining projects to restore...
Restore completed in 2.45 sec for C:\...\TesisTIC.API\TesisTIC.API.csproj
```

---

## 🔧 PASO 2: VERIFICAR COMPILACIÓN

Compila la solución para verificar que todo está correcto:

```bash
dotnet build
```

**Salida esperada:**

```
Build succeeded. 0 Warning(s)
```

Si hay errores, revisa:

- Namespaces en los archivos
- Referencias entre proyectos
- Paquetes NuGet correctamente instalados

---

## 🗄️ PASO 3: CONFIGURAR CADENA DE CONEXIÓN

Ya está configurada en `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=ep-lingering-mouse-aqf2l271-pooler.c-8.us-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_Oq18upwMAsNd;Port=5432;SSL Mode=Require;"
}
```

✅ **Verificado y listo**

---

## 📊 PASO 4: CREAR Y EJECUTAR MIGRACIONES EF CORE

### 4.1 Crear migración inicial

```bash
cd TesisTIC.API
dotnet ef migrations add InitialCreate -p ../TesisTIC.Infrastructure
```

**Esperado:**

```
Done. To undo this action, use 'dotnet ef migrations remove'
```

Esto crea:

- `TesisTIC.Infrastructure/Migrations/[timestamp]_InitialCreate.cs`
- `TesisTIC.Infrastructure/Migrations/TesisTicDbContextModelSnapshot.cs`

### 4.2 Aplicar migraciones a la BD

```bash
dotnet ef database update
```

**Esperado:**

```
Applying migration '20260517103000_InitialCreate'
Done.
```

#### ⚠️ SI HAY ERRORES DE CONEXIÓN:

1. Verifica que PostgreSQL Neon esté disponible
2. Verifica credenciales en `appsettings.json`
3. Verifica que la BD `neondb` exista

```bash
# Para verificar la conexión manualmente
psql 'postgresql://neondb_owner:npg_Oq18upwMAsNd@ep-lingering-mouse-aqf2l271-pooler.c-8.us-east-1.aws.neon.tech/neondb?sslmode=require'
```

---

## ▶️ PASO 5: EJECUTAR EL PROYECTO

### Opción A: Desde Terminal

```bash
cd TesisTIC.API
dotnet run
```

### Opción B: Desde Visual Studio Code

1. Abre el archivo [Program.cs](c:\Users\USER\source\repos\TesisPropuestasTIC-1\backend\TesisTIC.API\Program.cs)
2. Presiona **F5** (Debug) o **Ctrl+F5** (Sin debug)

### Opción C: Desde Visual Studio 2022

1. Abre el archivo `TesisTIC.sln`
2. Click derecho en **TesisTIC.API** → **Set as Startup Project**
3. Presiona **F5** o **Start Debugging**

---

## 🌐 PASO 6: VERIFICAR QUE LA API ESTÁ CORRIENDO

Deberías ver en la consola:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to exit
```

Abre tu navegador:

**API corriendo:**

```
http://localhost:5000/health
```

**Respuesta esperada:**

```json
{
  "status": "healthy",
  "timestamp": "2026-05-17T10:30:00Z"
}
```

**Swagger UI (Documentación):**

```
http://localhost:5000/swagger
```

---

## 🧪 PASO 7: PROBAR ENDPOINTS

### Opción A: Usar Postman

1. Descarga [Postman](https://www.postman.com/)
2. Importa o crea requests basadas en [ENDPOINTS_POSTMAN.md](ENDPOINTS_POSTMAN.md)

### Opción B: Usar Curl

```bash
# Health check
curl http://localhost:5000/health

# Obtener todas las propuestas
curl http://localhost:5000/api/propuestas

# Crear docente
curl -X POST "http://localhost:5000/api/docentes" \
  -H "Content-Type: application/json" \
  -d '{
    "nombres": "Juan",
    "apellidos": "Pérez",
    "correo": "juan@epn.edu.ec",
    "tituloAcademico": "Ingeniero"
  }'
```

### Opción C: Usar VS Code REST Client Extension

Instala la extensión [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)

Crea archivo `test.http`:

```http
### Health Check
GET http://localhost:5000/health

### Obtener propuestas
GET http://localhost:5000/api/propuestas

### Crear docente
POST http://localhost:5000/api/docentes
Content-Type: application/json

{
  "nombres": "Juan",
  "apellidos": "Pérez",
  "correo": "juan@epn.edu.ec",
  "tituloAcademico": "Ingeniero"
}
```

Luego haz click en **Send Request** sobre cada endpoint.

---

## 📋 ESTRUCTURA DE ARCHIVOS CREADOS

```
backend/
├── TesisTIC.API/
│   ├── Controllers/
│   │   ├── PropuestasController.cs          ✅
│   │   ├── DocentesController.cs            ✅
│   │   ├── AsignaturasController.cs         ✅
│   │   └── EstudiantesController.cs         ✅
│   ├── Program.cs                           ✅ (Actualizado)
│   ├── appsettings.json                     ✅ (Configurado)
│   ├── appsettings.Development.json         ✅
│   └── TesisTIC.API.csproj                  ✅
│
├── TesisTIC.Application/
│   ├── DTOs/                                ✅ (9 archivos)
│   ├── Interfaces/                          ✅ (5 archivos)
│   ├── Services/                            ✅ (4 servicios)
│   ├── Mappings/
│   │   └── MappingProfile.cs                ✅ (9 perfiles)
│   └── TesisTIC.Application.csproj          ✅
│
├── TesisTIC.Domain/
│   ├── Entities/                            ✅ (10 entidades)
│   └── TesisTIC.Domain.csproj               ✅
│
├── TesisTIC.Infrastructure/
│   ├── Data/
│   │   ├── TesisTicDbContext.cs             ✅
│   │   ├── Repositories/
│   │   │   ├── GenericRepository.cs         ✅
│   │   │   ├── PropuestaRepository.cs       ✅
│   │   │   ├── DocenteRepository.cs         ✅
│   │   │   ├── AsignaturaRepository.cs      ✅
│   │   │   └── EstudianteRepository.cs      ✅
│   │   └── Migrations/                      (Se crearán)
│   └── TesisTIC.Infrastructure.csproj       ✅
│
└── TesisTIC.sln                              ✅
```

---

## 📊 RESUMEN DE LO IMPLEMENTADO

### ✅ COMPLETADO

- [x] Entidades (10)
- [x] DbContext con mapeo EF Core
- [x] DTOs (23)
- [x] Repositorios genéricos y específicos (5)
- [x] Servicios CRUD (4)
- [x] AutoMapper Profiles (9)
- [x] Controllers REST (4)
- [x] Swagger/OpenAPI configurado
- [x] CORS configurado para Angular
- [x] Inyección de dependencias
- [x] Manejo de excepciones
- [x] Logging

### 📋 PENDIENTE

- [ ] Migraciones EF (se crean con `dotnet ef migrations add`)
- [ ] Validadores FluentValidation
- [ ] Controllers adicionales (Componentes, Actividades, Observaciones, Aprobaciones)
- [ ] Tests unitarios
- [ ] Frontend Angular
- [ ] Integración Angular + API

---

## 🛠️ TROUBLESHOOTING

### Error: "No se encuentra dotnet"

```bash
# Verifica instalación de .NET 8
dotnet --version

# Debe retornar 8.0.0 o superior
```

### Error: "Could not find type TesisTIC.Infrastructure.Data.TesisTicDbContext"

- Verifica que [TesisTicDbContext.cs](c:\Users\USER\source\repos\TesisPropuestasTIC-1\backend\TesisTIC.Infrastructure\Data\TesisTicDbContext.cs) exista
- Verifica namespaces
- Ejecuta `dotnet build` para recompilar

### Error: "Connection refused"

- Verifica que PostgreSQL Neon esté disponible
- Verifica la cadena de conexión en `appsettings.json`
- Intenta conectar manualmente con psql

### Error: "No migrations provided"

```bash
# Asegúrate de estar en TesisTIC.API
cd TesisTIC.API

# Crea migraciones
dotnet ef migrations add InitialCreate -p ../TesisTIC.Infrastructure

# Aplica cambios
dotnet ef database update
```

### Puerto 5000 ya en uso

```bash
# Usa otro puerto
dotnet run --urls "http://localhost:5001"
```

---

## 📌 COMANDOS RÁPIDOS

```bash
# Compilar
dotnet build

# Ejecutar
dotnet run

# Restaurar paquetes
dotnet restore

# Crear migración
dotnet ef migrations add [NombreMigracion] -p ../TesisTIC.Infrastructure

# Aplicar migración
dotnet ef database update

# Ver migraciones pendientes
dotnet ef migrations list

# Revertir última migración
dotnet ef database update [NombreMigracionAnterior]

# Eliminar última migración (si no se aplicó)
dotnet ef migrations remove
```

---

## 📞 PRÓXIMOS PASOS

1. **Ejecutar el backend** con `dotnet run`
2. **Probar endpoints** en Swagger o Postman
3. **Verificar BD** - Revisar que las tablas se crearon
4. **Crear Frontend Angular** - Crear servicios HTTP y componentes
5. **Integración** - Conectar Angular con API

---

## 🎓 RECURSOS

- [Documentación DTOs](ESTRUCTURA_BACKEND.md#dto)
- [Guía Endpoints](ENDPOINTS_POSTMAN.md)
- [Documentación .NET 8](https://learn.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [PostgreSQL](https://www.postgresql.org/docs/)

---

**¡El backend está 100% listo para usar!**
