# Guías de Uso del Sistema

## Documentación Disponible

### 1. [Guía Completa de Testing](./TESTING.md)
- Orden recomendado de pruebas por HU
- Casos de prueba detallados (25+ casos)
- Datos de ejemplo
- Checklist de validación
- Performance esperado

**Leído por**: QA Engineers, Desarrolladores

---

### 2. [Colección Postman](../postman/TesisTIC_Collection.json)
- Todos los endpoints (42+) organizados por HU
- Request/Response examples
- Test scripts automáticos
- Variables de entorno

**Leído por**: Desarrolladores, QA, API Testers

---

### 3. [Documentación de Flujos](../flujos/README.md)
- Diagrama textual de estados
- Flujos por Historia de Usuario
- Matrices de transiciones
- Consideraciones de diseño

**Leído por**: Product Managers, Desarrolladores

---

### 4. [Documentación de Seguridad](../seguridad/README.md)
- Endpoints públicos vs protegidos
- Validaciones por estado
- Protecciones contra XSS, SQL Injection, CORS
- Recomendaciones de seguridad

**Leído por**: DevOps, Security, Arquitectos

---

## Flujos de Trabajo Rápidos

### ✅ Tengo una propuesta en BORRADOR, quiero enviar a revisión

1. Completa todo (título, descripción, objetivo, alcance, asignaturas)
2. Click botón "Enviar a Revisión"
3. Confirma en modal
4. Propuesta pasa a PENDIENTE
5. Ya está en tablero de CPGIC

---

### ✅ Tengo observaciones, quiero editar y reenviar

1. Ve las observaciones en detalle de propuesta (estado OBSERVADA)
2. Click botón "Editar Propuesta"
3. Realiza correcciones
4. Click "Guardar"
5. Click botón "Reenviar Después de Correcciones"
6. Propuesta vuelve a PENDIENTE (espera nueva revisión)

---

### ✅ Propuesta APROBADA, quiero asignar estudiantes

1. Abre detalle de propuesta en estado APROBADA
2. Click botón "Asignar Estudiantes"
3. Modal abre con buscador
4. Selecciona hasta 5 estudiantes
5. Escribe motivo
6. Click "Asignar"
7. Propuesta automáticamente pasa a PENDIENTE (solicita nueva revisión)

---

### ✅ Propuesta está APROBADA con estudiantes, requiere última aprobación

1. En detalle ve botón "Solicitar Nueva Aprobación"
2. Click botón
3. Escribe motivo
4. Confirma
5. Propuesta se mantiene PENDIENTE (CPGIC revisa definitivamente)

---

### ✅ Quiero eliminar una propuesta

1. Abre propuesta en estado **BORRADOR**
2. Click botón "Eliminar Propuesta"
3. Lee advertencia: "NO se puede deshacer"
4. Click "Confirmar"
5. Propuesta eliminada
6. Se redirige a tablero

**Nota**: Solo BORRADOR puede eliminarse. Otros estados: NO permitido.

---

## Cómo Usar la Colección Postman

### Setup Inicial

1. Descargar Postman: https://www.postman.com/downloads/
2. Abrir Postman
3. Menu: File > Import
4. Seleccionar: `/postman/TesisTIC_Collection.json`
5. Importará todas las carpetas de HUs

### Configurar Variables

1. Tab "Variables" (esquina superior derecha)
2. Editar variable `baseUrl`:
   - **Desarrollo**: `http://localhost:5000/api`
   - **Producción**: `https://api.testistic.com/api`

### Ejecutar un Request

1. Expandir carpeta HU (e.g., "HU01 - Registro")
2. Click en request (e.g., "1.1 - Crear Propuesta")
3. Click botón azul "Send"
4. Ver respuesta en panel derecho
5. Validar Status Code, Response Body

### Auto-guardar IDs

Después de crear propuesta:
```
1. Se ejecuta test script automático
2. Guarda "propuestaId" en variable
3. Otros requests pueden usar: {{propuestaId}}
4. Ej: GET /api/propuestas/{{propuestaId}}
```

### Ejecutar Flujo Completo

1. Menu: Runner (esquina superior izquierda)
2. Seleccionar carpeta (e.g., "HU01")
3. Click "Run HU01"
4. Se ejecutan todos los requests en orden
5. Genera reporte con pases/fallos

---

## Errores Comunes y Soluciones

### ❌ "Cannot GET /api/propuestas" (404)

**Causa**: Backend no está ejecutándose

**Solución**:
```bash
cd backend
dotnet run
# Debe decir: "Now listening on: http://localhost:5000"
```

---

### ❌ "No se puede editar propuesta en estado PENDIENTE"

**Causa**: Intentas editar propuesta que ya envió a revisión

**Solución**:
- Si necesita cambios → espera a que sea OBSERVADA
- O crea propuesta nueva

---

### ❌ "No se pueden asignar más de 5 estudiantes"

**Causa**: Excediste límite de 5 estudiantes

**Solución**:
- Selecciona máximo 5 estudiantes
- Postman muestra error 400 con mensaje claro

---

### ❌ "No se puede eliminar propuesta en estado PENDIENTE"

**Causa**: Solo BORRADOR puede eliminarse

**Solución**:
- Una vez enviada a revisión no se puede eliminar
- Considera si realmente deseas eliminar (Postman tiene test que previene error)

---

### ❌ Response vacío (200 OK sin body)

**Causa**: Algunos DELETE retornan 204 No Content

**Solución**: Normal, significa éxito sin body. Postman muestra "Status 204"

---

## Métricas de Éxito

Después de completar testing:

| Métrica | Objetivo | Fórmula |
|---------|----------|---------|
| **Cobertura de Casos** | 100% | Casos pasados / Total casos |
| **Performance GET** | < 500ms | Promedio respuestas GET |
| **Performance POST** | < 500ms | Promedio respuestas POST |
| **Tasa de Error** | 0% | Errores inesperados / Total |

---

## Próximos Pasos

### Para Producción

1. ✅ Completar testing (esta guía)
2. ⏳ Implementar autenticación (OAuth2)
3. ⏳ Ejecutar security audit
4. ⏳ Load testing (100+ users simultáneos)
5. ⏳ Deploy a servidor producción
6. ⏳ Monitoreo en vivo (logs, métricas)

### Para Futuras Iteraciones

- [ ] Notificaciones por email
- [ ] Exportar propuestas a PDF
- [ ] Dashboard analítico (statistics)
- [ ] Búsqueda avanzada con filtros
- [ ] Integración con sistema de calificaciones

---

## Contacto y Soporte

**¿Problemas con la API?** → Ver [Documentación de Endpoints](../endpoints/README.md)  
**¿No entiendes un flujo?** → Ver [Flujos del Sistema](../flujos/README.md)  
**¿Dudas de seguridad?** → Ver [Seguridad](../seguridad/README.md)  
**¿Error en testing?** → Ver sección "Errores Comunes" arriba  

---

**Versión**: 1.0  
**Actualización**: Mayo 2026  
**Mantenedor**: TesisTIC Team
