# 🚀 Cómo Ejecutar el Sistema TIC

## ✅ Requisitos Previos

- **Backend**: .NET 8 SDK instalado
- **Frontend**: Node.js 18+ y npm instalados
- **Base de Datos**: PostgreSQL en la nube (ya configurada)

---

## 📋 Opción 1: Ejecutar TODO automáticamente (Recomendado)

### En Windows PowerShell:

```powershell
# Navega a la carpeta del proyecto
cd C:\Users\USER\source\repos\TesisPropuestasTIC-1

# Ejecuta el script de inicio
.\start-dev.ps1
```

**Esto inicia automáticamente:**

- ✅ Backend (.NET) en `http://localhost:5000`
- ✅ Frontend (Angular) en `http://localhost:4200`

---

## 🔧 Opción 2: Ejecutar Manualmente

### Paso 1: Terminal 1 - Backend (.NET)

```powershell
cd C:\Users\USER\source\repos\TesisPropuestasTIC-1\backend\TesisTIC.API

# Restaurar dependencias
dotnet restore

# Ejecutar el servidor
dotnet run
```

**Resultado esperado:**

```
Now listening on: http://localhost:5000
Now listening on: https://localhost:5001
```

### Paso 2: Terminal 2 - Frontend (Angular)

```powershell
cd C:\Users\USER\source\repos\TesisPropuestasTIC-1\frontend\tesis-tic-app

# Instalar dependencias (primer intento)
npm install

# Iniciar servidor de desarrollo
npm start
```

**Resultado esperado:**

```
✔ Compiled successfully.
Application bundle generation complete. [X seconds]
Watch mode enabled. Watching for file changes...
```

---

## 🌐 Acceder a los Servidores

Después de ejecutar, abre tu navegador:

- **Frontend (Aplicación)**: http://localhost:4200
- **Backend API**: http://localhost:5000/api
- **Swagger API Docs**: http://localhost:5000/swagger (próximamente)

---

## 📊 Estado de Conexiones

### Base de Datos

- **Servidor**: PostgreSQL en Render
- **Host**: `dpg-d73t496uk2gs739r5iag-a.oregon-postgres.render.com`
- **Base**: `testistic_jdep`
- **Estado**: ✅ Conectada

### API Backend

- **Framework**: ASP.NET Core 8
- **Puerto**: 5000 (HTTP) / 5001 (HTTPS)
- **Estado**: ✅ Corriendo

### Frontend Angular

- **Framework**: Angular 17
- **Puerto**: 4200
- **Estado**: ✅ Corriendo

---

## 🐛 Solucionar Problemas

### "Port already in use"

```powershell
# Buscar proceso en el puerto
Get-NetTCPConnection -LocalPort 5000 | Stop-Process -Force

# O cambiar puerto en Program.cs
dotnet run --urls=http://localhost:5002
```

### Angular no compila

```powershell
# Limpiar cache
rm -r node_modules package-lock.json

# Reinstalar
npm install
npm start
```

### Base de datos no conecta

- Verifica conexión a internet
- Revisa credenciales en `appsettings.json`
- Verifica firewall permita conexiones Postgres

---

## 📁 Estructura del Proyecto

```
TesisPropuestasTIC-1/
├── backend/
│   ├── TesisTIC.API/          ← API REST (.NET)
│   ├── TesisTIC.Application/   ← Lógica de negocio
│   ├── TesisTIC.Domain/        ← Entidades
│   └── TesisTIC.Infrastructure/← Acceso a datos
├── frontend/
│   └── tesis-tic-app/          ← Angular App
│       ├── src/
│       │   ├── app/
│       │   │   ├── components/
│       │   │   ├── services/
│       │   │   └── app-routing.module.ts
│       │   └── assets/
│       │       ├── images/
│       │       └── logos/      ← Logos (EPN)
│       └── angular.json
├── database/
│   ├── init.sql
│   └── migrations/
└── start-dev.ps1               ← Script inicio automático
```

---

## 🎯 Funcionalidades Implementadas

✅ **Backend**

- CRUD completo de propuestas
- Gestión de módulos y actividades
- Base de datos PostgreSQL
- API REST con endpoints

✅ **Frontend**

- Dashboard con listado de propuestas
- Creación/Edición de propuestas
- Asignación de estudiantes
- Detalle de propuestas
- Interfaz responsiva

---

## 📝 Notas Importantes

- **Sin Login**: La aplicación va directo al dashboard
- **Datos de Prueba**: Se cargan automáticamente desde la BD
- **CSS Variables**: Usa temas con --navy, --gold, --g800, etc.
- **API CORS**: Configurado para localhost:4200

---

**¡Sistema listo para usar! 🎉**
