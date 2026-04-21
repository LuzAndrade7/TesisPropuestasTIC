# 🎓 Sistema TIC — EPN | Guía de Ejecución

## ✨ Lo Nuevo

✅ **Interfaz Profesional Completa** — Dashboard con modal de eliminación, tabla de propuestas y estadísticas  
✅ **Diseño Premium** — Colores, tipografía y espaciados de clase mundial  
✅ **Integración Backend** — Conecta automáticamente con tu API en .NET (localhost:5000)  
✅ **Responsivo** — Se adapta a cualquier dispositivo

---

## 🚀 OPCIÓN 1: INICIO AUTOMÁTICO (Recomendado ⭐)

### Abre PowerShell como ADMINISTRADOR y copia esto:

```powershell
cd C:\Users\USER\source\repos\TesisPropuestasTIC-1
.\start-dev.ps1
```

**Esto hace automáticamente:**

- ✅ Backend .NET corre en http://localhost:5000
- ✅ Frontend Angular sirve archivos en http://localhost:4200

---

## 🔧 OPCIÓN 2: ABRIR LA APP DIRECTA (Sin Angular)

**Forma más rápida de ver la interfaz:**

1. Abre PowerShell
2. Ejecuta el backend:

```powershell
cd C:\Users\USER\source\repos\TesisPropuestasTIC-1\backend\TesisTIC.API
dotnet run
# Espera a que vea: "Now listening on: http://localhost:5000"
```

3. Luego abre en tu navegador:

```
file:///C:/Users/USER/source/repos/TesisPropuestasTIC-1/frontend/Sistema-TIC.html
```

**O simplemente:**

- Ctrl+O en el navegador
- Busca: `Sistema-TIC.html`
- Click en él

---

## 📊 ¿QUÉ SE VE?

Cuando abres Sistema-TIC.html:

```
┌─────────────────────────────────────────┐
│ FACULTAD DE INGENIERÍA DE SISTEMAS — EPN│
│                DASHBOARD                 │
├─────────────────────────────────────────┤
│ Mis propuestas                           │
│ [ 5 TODOS ] [ 1 BORRADOR ] ...          │
├─────────────────────────────────────────┤
│ N° │ TÍTULO         │ ESTADO  │ ACCIONES │
├─────────────────────────────────────────┤
│  1  │ Sistema TIC    │ ✓ Obs. │ 👁 🗑   │
│  2  │ App Móvil      │ ⏩ Pend│ 👁      │
│  ...                                     │
└─────────────────────────────────────────┘
```

---

## 🌐 URLs DE ACCESO

| Componente                 | URL                                                                                 | Estatus      |
| -------------------------- | ----------------------------------------------------------------------------------- | ------------ |
| **Interfaz (Recomendada)** | `file:///C:/Users/USER/source/repos/TesisPropuestasTIC-1/frontend/Sistema-TIC.html` | ✅ Lista     |
| **API Backend**            | `http://localhost:5000/api/propuestas`                                              | ✅ Corriendo |
| **Base de Datos**          | Render Cloud (PostgreSQL)                                                           | ✅ Conectada |
| **Angular (Alternativa)**  | `http://localhost:4200`                                                             | ⚙️ Opcional  |

---

## 📝 PASOS DETALLADOS

### PASO 1️⃣: Abre Terminal 1 (BACKEND)

```powershell
cd C:\Users\USER\source\repos\TesisPropuestasTIC-1\backend\TesisTIC.API
dotnet restore
dotnet run
```

**Espera a ver esto:**

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
```

✅ **Backend Corriendo**

---

### PASO 2️⃣: Abre Tu Navegador (FRONTEND)

Abre **Navegador** (Chrome, Firefox, Edge, etc.)

**Opción A — Interfaz Profesional:**

Presiona `Ctrl+O` y busca:

```
C:\Users\USER\source\repos\TesisPropuestasTIC-1\frontend\Sistema-TIC.html
```

**O directamente en la barra:**

```
file:///C:/Users/USER/source/repos/TesisPropuestasTIC-1/frontend/Sistema-TIC.html
```

✅ **¡Verás el dashboard!**

**Opción B — Angular (si prefieres):**

Si ejecutaste `.\start-dev.ps1`, abre:

```
http://localhost:4200
```

---

## 🎯 FUNCIONALIDADES DISPONIBLES

✅ **Dashboard** — Lista de 5 propuestas con filtros por estado  
✅ **Estadísticas** — Contador de propuestas por estado  
✅ **Modal de Eliminación** — Profesional como la imagen que compartiste  
✅ **Tabla Responsiva** — Datos se cargan de la API  
✅ **Integración Backend** — Lee datos reales de tu BD

---

## 🔌 CONEXIÓN CON EL BACKEND

El archivo `Sistema-TIC.html` busca automáticamente las propuestas en:

```
GET http://localhost:5000/api/propuestas
```

**Asegúrate que:**

- ✅ Backend está corriendo en puerto 5000
- ✅ Tienes propuestas en la BD (se cargan automáticamente)
- ✅ CORS está habilitado en Program.cs (ya está configurado)

---

## ❌ SI NO FUNCIONA

### "No carga las propuestas"

```
✔ Abre la consola (F12 → Console)
✔ Busca errores
✔ Asegúrate que el backend está corriendo
✔ Verifica http://localhost:5000/api/propuestas en el navegador
```

### "Puerto 5000 en uso"

```powershell
Get-NetTCPConnection -LocalPort 5000 | Stop-Process -Force
```

### "No puedo abrir el HTML"

```powershell
# Ve a la carpeta
cd C:\Users\USER\source\repos\TesisPropuestasTIC-1\frontend

# Abre el archivo
Start-Process Sistema-TIC.html
```

---

## 📁 ESTRUCTURA POST-SETUP

```
TesisPropuestasTIC-1/
├── backend/                    ← API .NET (puerto 5000)
│   └── TesisTIC.API/
├── frontend/
│   ├── Sistema-TIC.html       ← 🎯 ABRE ESTO (interfaz)
│   ├── tesis-tic-app/         ← Angular (opcional)
│   └── assets/
│       └── logos/epn-logo.svg
├── database/                   ← Migraciones SQL
├── start-dev.ps1              ← Script automático
└── EJECUTAR.md                ← Esta guía
```

---

## ✨ CARACTERÍSTICAS DEL DISEÑO

- **Tipografía:** Montserrat (profesional)
- **Colores:** Navy, Gold, Red (EPN)
- **Componentes:** Modales, tablas, badges
- **Animaciones:** Suaves y fluidas
- **Responsividad:** Funciona en móvil/tablet

---

## 🎬 RESUMEN RÁPIDO

| Acción           | Comando                                 |
| ---------------- | --------------------------------------- |
| **Ver interfaz** | `file:///.../Sistema-TIC.html`          |
| **Iniciar todo** | `.\start-dev.ps1`                       |
| **Backend solo** | `cd backend/TesisTIC.API && dotnet run` |
| **Chequear API** | `http://localhost:5000/api/propuestas`  |
| **Ver logs**     | Presiona F12 en el navegador            |

---

## 🏆 ¡Sistema Listo!

**Backend:** ✅ Corriendo  
**Frontend:** ✅ Profesional  
**Base de Datos:** ✅ Conectada  
**Interfaz:** ✅ Premium

**¡A trabajar!** 🚀
