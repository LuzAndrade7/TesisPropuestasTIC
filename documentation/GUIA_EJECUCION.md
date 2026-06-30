# Guia de Ejecucion - TesisTIC

Esta guia levanta la version final local del sistema.

## Requisitos

- .NET SDK 8
- Node.js compatible con Angular 17
- npm
- Git
- Acceso a internet para Neon

## Backend

Desde la raiz del repositorio:

```bash
cd backend
dotnet restore
dotnet build TesisTIC.sln
dotnet run --project TesisTIC.API
```

Swagger queda disponible en:

```text
http://localhost:5000/swagger
```

Si el backend abre otro puerto, revisar la salida de consola de ASP.NET.

## Frontend

Desde la raiz del repositorio:

```bash
cd frontend
npm install
npm start
```

La aplicacion queda disponible en:

```text
http://localhost:4200
```

Si el puerto 4200 esta ocupado:

```bash
npm start -- --port 4201
```

## Base de Datos

Por seguridad, la credencial real no debe guardarse en Git. Los archivos versionados dejan la cadena vacia:

- `backend/TesisTIC.API/appsettings.json`
- `backend/TesisTIC.API/appsettings.Development.json`

La aplicacion lee la conexion desde cualquiera de estas opciones:

### Opcion A: variable de entorno

PowerShell:

```powershell
$env:ConnectionStrings__TesisTICConnection="Host=YOUR_NEON_HOST;Database=YOUR_DATABASE;Username=YOUR_USERNAME;Password=YOUR_PASSWORD;Port=5432;SSL Mode=Require;Channel Binding=Require;"
```

### Opcion B: archivo local ignorado por Git

Copiar:

```text
backend/TesisTIC.API/appsettings.Local.example.json
```

como:

```text
backend/TesisTIC.API/appsettings.Local.json
```

y colocar ahi la cadena real. `appsettings.Local.json` esta ignorado por Git.

### Opcion C: user-secrets de .NET

```bash
cd backend/TesisTIC.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:TesisTICConnection" "Host=YOUR_NEON_HOST;Database=YOUR_DATABASE;Username=YOUR_USERNAME;Password=YOUR_PASSWORD;Port=5432;SSL Mode=Require;Channel Binding=Require;"
```

La cadena de Neon debe usar:

```text
SSL Mode=Require;Channel Binding=Require;
```

Para verificar manualmente con `psql` si esta instalado, usa el valor real de tu cadena:

```bash
psql "postgresql://USER:PASSWORD@HOST/DATABASE?sslmode=require&channel_binding=require"
```

## Scripts SQL

Los scripts se encuentran en `database/`:

- `sprint1_init.sql`
- `sprint2_observaciones.sql`
- `sprint3_historial_estados.sql`
- `sprint4_propuesta_estudiantes.sql`
- `Setup_Propuesta_Observada.sql`

Ejecutarlos en orden si se necesita reconstruir una base desde cero.

## Prueba Rapida

1. Abrir frontend en `/tablero`.
2. Crear propuesta nueva.
3. Ingresar datos generales y asignaturas.
4. Configurar 2 a 5 participantes.
5. Registrar un modulo por participante.
6. En actividades, intentar superar 288 horas en un modulo.
7. Verificar que aparece el modal de maximo permitido.
8. Guardar borrador.
9. Enviar a revision.
10. Revisar el detalle de la propuesta.

## Verificacion de Build

Backend:

```bash
cd backend
dotnet build TesisTIC.sln
```

Frontend:

```bash
cd frontend
npm run build
```

Advertencias conocidas:

- Angular puede advertir que el bundle inicial excede el presupuesto configurado.
- NuGet puede advertir sobre versiones o vulnerabilidades de paquetes.

Si no hay errores, la version compila correctamente.

## Problemas Comunes

Puerto ocupado:

```bash
dotnet run --project TesisTIC.API --urls "http://localhost:5001"
npm start -- --port 4201
```

No conecta a Neon:

- Confirmar internet.
- Confirmar que la rama de Neon no este archivada.
- Revisar host, usuario, password, base y SSL.
- Intentar nuevamente; Neon puede tardar al despertar una rama archivada.

Frontend no consume API:

- Revisar `frontend/src/environments/environment.ts`.
- Confirmar que el backend este corriendo.
- Confirmar CORS en `appsettings`.
