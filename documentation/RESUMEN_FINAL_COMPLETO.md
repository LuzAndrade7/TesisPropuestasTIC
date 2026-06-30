# Resumen Final Completo - TesisTIC

Fecha: junio 2026  
Estado: version final funcional

## Objetivo

Implementar un sistema para gestionar propuestas TIC, permitiendo que profesores registren propuestas, CPGIC las revise, se gestionen observaciones, se asignen estudiantes y se mantenga control de estados.

## Resultado

El sistema queda completo para el flujo principal:

1. Crear propuesta.
2. Guardar borrador.
3. Editar mientras este en BORRADOR u OBSERVADA.
4. Enviar a revision.
5. Recibir observaciones.
6. Corregir y reenviar.
7. Consultar detalle.
8. Aprobar o cambiar estado desde backend.
9. Asignar estudiantes.
10. Solicitar nueva aprobacion si corresponde.
11. Eliminar solo borradores.

## Backend

Proyectos:

- `TesisTIC.API`
- `TesisTIC.Application`
- `TesisTIC.Domain`
- `TesisTIC.Infrastructure`

Responsabilidades:

- Controllers REST.
- Servicios con reglas de negocio.
- Repositorios EF Core.
- DTOs para entrada/salida.
- Entidades de dominio.
- DbContext y configuracion PostgreSQL.

Controllers principales:

- `PropuestasController`
- `ObservacionesController`
- `EstudiantesController`
- `DocentesController`
- `AsignaturasController`

Servicios principales:

- `PropuestaService`
- `PropuestaDetalleService`
- `PropuestaEstudianteService`
- `ObservacionesCpgicService`
- `EstudianteService`
- `DocenteService`
- `AsignaturaService`

## Frontend

Componentes principales:

- `tablero`: listado, filtros, acciones y modales de envio/eliminacion.
- `formulario-propuesta`: creacion, edicion, observaciones, modulos y validaciones.
- `detalle-propuesta`: consulta completa y acciones por estado.
- `asignar-estudiantes`: asignacion de estudiantes por modulo.

Mejoras finales del formulario:

- Numero de participantes limitado de 2 a 5.
- Sincronizacion de modulos al aumentar participantes.
- Modal informativo para cantidades excepcionales de participantes.
- Validacion de modulos antes de enviar o reenviar.
- Maximo de 288 horas por modulo.
- Modal cuando se intenta superar el maximo de horas.
- Ajuste automatico al cupo restante de horas.
- Boton volver/cancelar segun estado de guardado.

## Base de Datos

Se usan scripts por sprint:

- `sprint1_init.sql`: esquema inicial.
- `sprint2_observaciones.sql`: observaciones CPGIC.
- `sprint3_historial_estados.sql`: historial.
- `sprint4_propuesta_estudiantes.sql`: asignacion de estudiantes.
- `Setup_Propuesta_Observada.sql`: datos para pruebas de observaciones.

## Reglas de Negocio

Estados:

- BORRADOR
- PENDIENTE
- OBSERVADA
- APROBADA
- RECHAZADA

Reglas:

- Solo BORRADOR se puede eliminar.
- BORRADOR y OBSERVADA se pueden editar.
- PENDIENTE no se edita desde el formulario del profesor.
- OBSERVADA puede reenviarse tras correcciones.
- APROBADA permite asignacion de estudiantes.
- Cambios posteriores en estudiantes pueden requerir nueva aprobacion.
- La propuesta debe tener tantos modulos como participantes.
- Cada modulo no puede superar 288 horas.

## Validaciones de Usabilidad

El sistema usa modales y mensajes consistentes para:

- Confirmar envio.
- Confirmar reenvio.
- Confirmar salida sin guardar.
- Confirmar eliminacion.
- Avisar limite de participantes.
- Avisar limite de 288 horas por modulo.

La validacion de 288 horas no espera al envio: se aplica durante la captura de actividades para evitar que el usuario acumule un total invalido.

## Verificacion

Comandos usados:

```bash
cd backend
dotnet build TesisTIC.sln
```

```bash
cd frontend
npm run build
```

Resultado:

- Backend: compilacion correcta.
- Frontend: compilacion correcta.
- Advertencias no bloqueantes: presupuesto de bundle Angular y avisos de paquetes.

## Estado Final

La version actual esta lista para llevarse a `main`. Para produccion real faltarian endurecimientos como autenticacion, roles, manejo de secretos y despliegue formal, pero para la entrega funcional del sistema el flujo principal esta completo.
