# Guía de Inicio Rápido

## Inicio Rápido en 5 minutos

### Opción 1: Con Docker (Recomendado para desarrollo rápido)

```bash
# Clonar el repositorio
git clone <repository-url>
cd TesisPropuestasTIC-1

# Cambiar a rama ModuloA
git checkout ModuloA

# Ejecutar docker-compose
docker-compose up
```

### Opción 2: Instalación Manual

#### Paso 1: Preparar Base de Datos

```bash
# Asumiendo PostgreSQL instalado y ejecutándose
psql -U postgres < database/init.sql
```

#### Paso 2: Ejecutar Backend

```bash
cd backend
dotnet restore
dotnet run --project TesisTIC.API/TesisTIC.API.csproj
```

**Resultado esperado:**

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

#### Paso 3: Ejecutar Frontend

En otra terminal:

```bash
cd frontend/tesis-tic-app
npm install
npm start
```

**Resultado esperado:**

```
✔ Compiled successfully.

Applications are running on:
  http://localhost:4200
```

## Acceso a la Aplicación

- **Frontend:** http://localhost:4200
- **Swagger API:** http://localhost:5000/swagger
- **Base de Datos:** PostgreSQL en localhost:5432

## Verificación de Funcionalidad

### Test 1: Listar Propuestas

```bash
curl http://localhost:5000/api/propuestas
```

### Test 2: Listar Estados

```bash
curl http://localhost:5000/api/estados
```

### Test 3: Crear Propuesta

```bash
curl -X POST http://localhost:5000/api/propuestas \
  -H "Content-Type: application/json" \
  -d '{
    "titulo": "Propuesta Test",
    "descripcion": "Descripción test",
    "objetivo": "Objetivo test",
    "alcance": "Alcance test",
    "componentesActividadesProductos": "Componentes test",
    "docenteId": 1,
    "numeroParticipantes": 2,
    "departamento": "Ingeniería",
    "facultad": "Ingeniería de Sistemas",
    "asignaturasIds": []
  }'
```

## Troubleshooting

### Error: "Connection refused" a PostgreSQL

- Verificar que PostgreSQL está corriendo: `psql`
- Revisar credenciales en `backend/TesisTIC.API/appsettings.json`

### Error: "CORS error" en el navegador

- Verificar que el backend está corriendo en http://localhost:5000
- Revisar la configuración CORS en `Program.cs`

### Error: "Modulos no encontrados" en Angular

```bash
cd frontend/tesis-tic-app
rm -rf node_modules package-lock.json
npm install
```

## Próximos Pasos

1. Revisar [DESARROLLO.md](./DESARROLLO.md) para documentación completa
2. Explorar la estructura del proyecto
3. Realizar cambios según especificaciones
4. Hacer commit y push a la rama ModuloA
