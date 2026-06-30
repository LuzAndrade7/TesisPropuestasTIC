-- ============================================================================
-- SCRIPT SQL: Crear Propuesta OBSERVADA con Observaciones CPGIC
-- ============================================================================
-- Propósito: Prepara datos de prueba para HU04 T11-T12 (Visualización y reenvío)
-- Base de Datos: PostgreSQL Neon
-- Pasos: 1. Crea propuesta en BORRADOR
--        2. Crea asignatura asociada
--        3. Cambia propuesta a PENDIENTE (simula envío)
--        4. Cambia propuesta a OBSERVADA (simula revisión)
--        5. Crea 2-3 observaciones de CPGIC
-- ============================================================================

-- 1. BUSCAR O CREAR UN DOCENTE
-- Verificar si existe docente con email específico, si no crear uno
INSERT INTO docentes (nombre, email, departamento)
SELECT 'Dr. Juan García', 'juan.garcia@epn.edu.ec', 'Sistemas Informáticos'
WHERE NOT EXISTS (
    SELECT 1 FROM docentes 
    WHERE email = 'juan.garcia@epn.edu.ec'
);

-- Obtener ID del docente para usarlo después
-- (En DBeaver puedes ejecutar estas líneas por separado o usar variable)

-- 2. BUSCAR O CREAR ASIGNATURAS
INSERT INTO asignaturas (nombre, codigo_asignatura, descripcion)
VALUES 
    ('Sistemas de Información', 'SI-101', 'Diseño e implementación de sistemas de información'),
    ('Bases de Datos Avanzadas', 'BD-201', 'Arquitectura y optimización de bases de datos'),
    ('Ingeniería de Software', 'ISW-301', 'Procesos, patrones y prácticas de ingeniería de software')
ON CONFLICT (codigo_asignatura) DO NOTHING;

-- 3. CREAR PROPUESTA EN ESTADO BORRADOR
-- IMPORTANTE: Reemplaza 1 con el ID real del docente si es diferente
INSERT INTO propuestas (
    nombre_proyecto,
    numero_participantes,
    profesor_id,
    descripcion,
    objetivo,
    alcance,
    estado,
    fecha_creacion,
    fecha_envio_revision,
    fecha_actualizacion
)
SELECT 
    'Plataforma de Evaluación Competencias TIC',
    3,
    id,
    'Desarrollo de plataforma web para evaluar y monitorear competencias TIC en estudiantes de educación superior. Sistema integrado con campus virtual y generación de reportes.',
    'Crear herramienta que permita evaluación automática y continua de competencias TIC mejorando la pedagogía en EPN',
    'Escuela Politécnica Nacional - Facultad de Sistemas',
    'OBSERVADA'::text,
    CURRENT_TIMESTAMP - INTERVAL '5 days',
    CURRENT_TIMESTAMP - INTERVAL '3 days',
    CURRENT_TIMESTAMP
FROM docentes
WHERE email = 'juan.garcia@epn.edu.ec'
AND NOT EXISTS (
    SELECT 1 FROM propuestas 
    WHERE nombre_proyecto = 'Plataforma de Evaluación Competencias TIC'
)
RETURNING id;

-- 4. ASOCIAR ASIGNATURAS A LA PROPUESTA
-- NOTA: Ejecuta primero la consulta anterior para obtener el propuesta_id
-- Luego reemplaza {PROPUESTA_ID} con el ID obtenido
-- O ejecuta estas dos juntas si tu DB lo permite:

INSERT INTO propuesta_asignaturas (propuesta_id, asignatura_id)
SELECT 
    p.id,
    a.id
FROM propuestas p
CROSS JOIN asignaturas a
WHERE p.nombre_proyecto = 'Plataforma de Evaluación Competencias TIC'
  AND a.codigo_asignatura IN ('SI-101', 'BD-201', 'ISW-301')
  AND NOT EXISTS (
      SELECT 1 FROM propuesta_asignaturas 
      WHERE propuesta_id = p.id 
      AND asignatura_id = a.id
  );

-- 5. CREAR OBSERVACIONES CPGIC
-- IMPORTANTE: Estas observaciones ya tienen estado OBSERVADA establecido arriba
INSERT INTO observaciones_cpgic (propuesta_id, observacion, realizado_por, fecha_observacion)
SELECT 
    p.id,
    'El objetivo debe ser más específico y medible. Considera definir métricas concretas de evaluación de competencias.',
    'CPGIC',
    CURRENT_TIMESTAMP - INTERVAL '2 days'
FROM propuestas p
WHERE p.nombre_proyecto = 'Plataforma de Evaluación Competencias TIC'
AND NOT EXISTS (
    SELECT 1 FROM observaciones_cpgic 
    WHERE propuesta_id = p.id 
    AND observacion ILIKE '%objetivo%'
);

INSERT INTO observaciones_cpgic (propuesta_id, observacion, realizado_por, fecha_observacion)
SELECT 
    p.id,
    'Falta detallar las metodologías de evaluación. Especifica: rúbricas, escalas de desempeño, criterios de calificación.',
    'CPGIC',
    CURRENT_TIMESTAMP - INTERVAL '2 days' + INTERVAL '1 minute'
FROM propuestas p
WHERE p.nombre_proyecto = 'Plataforma de Evaluación Competencias TIC'
AND NOT EXISTS (
    SELECT 1 FROM observaciones_cpgic 
    WHERE propuesta_id = p.id 
    AND observacion ILIKE '%metodologías%'
);

INSERT INTO observaciones_cpgic (propuesta_id, observacion, realizado_por, fecha_observacion)
SELECT 
    p.id,
    'El alcance incluye 3 asignaturas. Confirma disponibilidad de recursos tecnológicos para integración con campus virtual.',
    'CPGIC',
    CURRENT_TIMESTAMP - INTERVAL '2 days' + INTERVAL '2 minutes'
FROM propuestas p
WHERE p.nombre_proyecto = 'Plataforma de Evaluación Competencias TIC'
AND NOT EXISTS (
    SELECT 1 FROM observaciones_cpgic 
    WHERE propuesta_id = p.id 
    AND observacion ILIKE '%alcance%'
);

-- ============================================================================
-- VERIFICACIÓN: Ejecuta estas consultas para confirmar todo quedó bien
-- ============================================================================

-- Ver la propuesta creada
SELECT 
    id,
    nombre_proyecto,
    estado,
    fecha_creacion,
    fecha_envio_revision,
    fecha_actualizacion
FROM propuestas
WHERE nombre_proyecto = 'Plataforma de Evaluación Competencias TIC'
ORDER BY fecha_creacion DESC
LIMIT 1;

-- Ver asignaturas asociadas
SELECT 
    p.nombre_proyecto,
    a.codigo_asignatura,
    a.nombre as nombre_asignatura
FROM propuestas p
JOIN propuesta_asignaturas pa ON p.id = pa.propuesta_id
JOIN asignaturas a ON pa.asignatura_id = a.id
WHERE p.nombre_proyecto = 'Plataforma de Evaluación Competencias TIC'
ORDER BY a.codigo_asignatura;

-- Ver observaciones CPGIC asociadas (IMPORTANTE para T11-T12)
SELECT 
    id,
    propuesta_id,
    observacion,
    realizado_por,
    fecha_observacion
FROM observaciones_cpgic
WHERE propuesta_id IN (
    SELECT id FROM propuestas 
    WHERE nombre_proyecto = 'Plataforma de Evaluación Competencias TIC'
)
ORDER BY fecha_observacion;

-- Verificar estado
SELECT 
    COUNT(*) as total_observaciones,
    (SELECT estado FROM propuestas WHERE nombre_proyecto = 'Plataforma de Evaluación Competencias TIC' LIMIT 1) as estado_propuesta
FROM observaciones_cpgic
WHERE propuesta_id IN (
    SELECT id FROM propuestas 
    WHERE nombre_proyecto = 'Plataforma de Evaluación Competencias TIC'
);

-- ============================================================================
-- SCRIPT ALTERNATIVO: Si necesitas limpiar todo después de pruebas
-- ============================================================================
/*
-- DESCOMENTA ESTO SOLO SI NECESITAS LIMPIAR

-- Obtener ID de la propuesta
DO $$
DECLARE
    prop_id INTEGER;
BEGIN
    SELECT id INTO prop_id FROM propuestas 
    WHERE nombre_proyecto = 'Plataforma de Evaluación Competencias TIC'
    LIMIT 1;
    
    IF prop_id IS NOT NULL THEN
        -- Eliminar observaciones
        DELETE FROM observaciones_cpgic WHERE propuesta_id = prop_id;
        
        -- Eliminar asignaturas asociadas
        DELETE FROM propuesta_asignaturas WHERE propuesta_id = prop_id;
        
        -- Eliminar propuesta
        DELETE FROM propuestas WHERE id = prop_id;
        
        RAISE NOTICE 'Propuesta % y datos asociados eliminados', prop_id;
    END IF;
END $$;
*/

-- ============================================================================
-- NOTAS IMPORTANTES
-- ============================================================================
-- 1. Este script es IDEMPOTENTE: Puedes ejecutarlo múltiples veces sin errores
-- 2. Las observaciones tienen fechas realistas (hace 2 días)
-- 3. El estado es directamente OBSERVADA (simula que pasó por PENDIENTE)
-- 4. Docente email: juan.garcia@epn.edu.ec (si no existe se crea)
-- 5. Asignaturas: SI-101, BD-201, ISW-301 (3 asignaturas)
-- 6. Las observaciones son específicas y realistas para educación superior
-- 7. Usar en desarrollo/testing SOLAMENTE, no en producción
-- ============================================================================
