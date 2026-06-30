-- ===== HU07 T20: TABLA DE ASIGNACIÓN DE ESTUDIANTES A PROPUESTAS =====
-- Sprint 4: Crear tabla para asignar estudiantes a propuestas aprobadas
-- Base de datos: PostgreSQL Neon
-- Idempotent: Usa IF NOT EXISTS para seguridad

-- 1. CREAR TABLA (SI NO EXISTE)
CREATE TABLE IF NOT EXISTS propuesta_estudiantes (
    id SERIAL PRIMARY KEY,
    propuesta_id INTEGER NOT NULL REFERENCES propuestas(id) ON DELETE CASCADE,
    estudiante_id INTEGER NOT NULL REFERENCES estudiantes(id) ON DELETE CASCADE,
    fecha_asignacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    asignado_por VARCHAR(200),
    estado VARCHAR(50) DEFAULT 'ACTIVO' NOT NULL CHECK (estado IN ('ACTIVO', 'COMPLETADO', 'RETIRADO')),
    UNIQUE(propuesta_id, estudiante_id) -- No permitir duplicados
);

-- 2. CREAR ÍNDICES PARA OPTIMIZAR QUERIES
CREATE INDEX IF NOT EXISTS idx_propuesta_estudiantes_propuesta_id ON propuesta_estudiantes(propuesta_id);
CREATE INDEX IF NOT EXISTS idx_propuesta_estudiantes_estudiante_id ON propuesta_estudiantes(estudiante_id);
CREATE INDEX IF NOT EXISTS idx_propuesta_estudiantes_estado ON propuesta_estudiantes(propuesta_id, estado);

-- 3. VERIFICAR QUE LA TABLA SE CREÓ CORRECTAMENTE
SELECT 
    table_name,
    column_name,
    data_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'public' AND table_name = 'propuesta_estudiantes'
ORDER BY ordinal_position;

-- 4. VERIFICAR ÍNDICES
SELECT indexname FROM pg_indexes WHERE tablename = 'propuesta_estudiantes';

-- 5. LIMPIEZA (COMENTADO - DESCOMENTA SOLO SI NECESITAS EMPEZAR DE CERO)
-- DELETE FROM propuesta_estudiantes;
-- DROP TABLE IF EXISTS propuesta_estudiantes CASCADE;
