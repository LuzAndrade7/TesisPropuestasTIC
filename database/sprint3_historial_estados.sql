-- =============================================================================
-- MIGRATION: Crear tabla historial_estados para HU06 T17
-- =============================================================================
-- Propósito: Registrar cada cambio de estado de propuestas para línea de tiempo
-- Compatibilidad: PostgreSQL Neon
-- =============================================================================

-- Crear tabla si no existe
CREATE TABLE IF NOT EXISTS historial_estados (
    id SERIAL PRIMARY KEY,
    propuesta_id INT NOT NULL,
    estado_anterior VARCHAR(50),
    estado_nuevo VARCHAR(50) NOT NULL,
    motivo TEXT,
    realizado_por VARCHAR(200),
    fecha_cambio TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    -- Constraints
    CONSTRAINT fk_historial_propuesta FOREIGN KEY (propuesta_id) 
        REFERENCES propuestas(id) ON DELETE CASCADE,
    CONSTRAINT check_estado_nuevo CHECK (estado_nuevo IN ('BORRADOR', 'PENDIENTE', 'OBSERVADA', 'APROBADA', 'RECHAZADA'))
);

-- Crear índices para queries rápidas
CREATE INDEX IF NOT EXISTS idx_historial_propuesta_id ON historial_estados(propuesta_id);
CREATE INDEX IF NOT EXISTS idx_historial_fecha ON historial_estados(fecha_cambio DESC);
CREATE INDEX IF NOT EXISTS idx_historial_estado_nuevo ON historial_estados(estado_nuevo);

-- Crear secuencia si no existe (para compatibilidad Neon)
CREATE SEQUENCE IF NOT EXISTS historial_estados_id_seq START WITH 1 INCREMENT BY 1;

-- =============================================================================
-- DATOS INICIALES: Insertar histórico para propuestas existentes
-- =============================================================================

-- Para cada propuesta en estado != BORRADOR, crear entrada de histórico
INSERT INTO historial_estados (propuesta_id, estado_anterior, estado_nuevo, motivo, realizado_por, fecha_cambio)
SELECT 
    p.id,
    'BORRADOR'::VARCHAR(50),
    p.estado,
    CASE 
        WHEN p.estado = 'PENDIENTE' THEN 'Enviada a revisión por propuesto'
        WHEN p.estado = 'OBSERVADA' THEN 'Retornada con observaciones de CPGIC'
        WHEN p.estado = 'APROBADA' THEN 'Aprobada por CPGIC'
        WHEN p.estado = 'RECHAZADA' THEN 'Rechazada por CPGIC'
        ELSE 'Cambio de estado'
    END,
    'SISTEMA',
    p.fecha_envio_revision
FROM propuestas p
WHERE p.estado != 'BORRADOR'
AND NOT EXISTS (
    SELECT 1 FROM historial_estados 
    WHERE propuesta_id = p.id 
    AND estado_nuevo = p.estado
)
ON CONFLICT DO NOTHING;

-- =============================================================================
-- VERIFICACIÓN
-- =============================================================================

-- Ver tabla creada
SELECT 
    column_name,
    data_type,
    is_nullable
FROM information_schema.columns
WHERE table_name = 'historial_estados'
ORDER BY ordinal_position;

-- Ver índices
SELECT indexname FROM pg_indexes WHERE tablename = 'historial_estados';

-- Ver datos insertados
SELECT 
    id,
    propuesta_id,
    estado_anterior,
    estado_nuevo,
    motivo,
    fecha_cambio
FROM historial_estados
ORDER BY fecha_cambio DESC
LIMIT 10;

-- Ver conteo por propuesta
SELECT 
    propuesta_id,
    COUNT(*) as total_cambios,
    MIN(fecha_cambio) as primer_cambio,
    MAX(fecha_cambio) as ultimo_cambio
FROM historial_estados
GROUP BY propuesta_id
ORDER BY propuesta_id;

-- =============================================================================
-- CLEANUP (si necesitas resetear): Descomenta para limpiar
-- =============================================================================

/*
DROP TABLE IF EXISTS historial_estados CASCADE;
DROP SEQUENCE IF EXISTS historial_estados_id_seq;
*/
