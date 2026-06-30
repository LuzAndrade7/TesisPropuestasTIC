-- =========================================
-- SPRINT 2: Migración para HU04
-- Observaciones y Corrección de Propuestas
-- Base de datos: Neon PostgreSQL
-- Fecha: May 17, 2026
-- =========================================

-- NOTA: Este script crea la tabla observaciones_cpgic que permite
-- registrar observaciones sobre propuestas en revisión.
-- Compatible con Entity Framework Core y la entidad ObservacionesCpgic.cs

-- =========================================
-- CREAR TABLA OBSERVACIONES_CPGIC
-- =========================================
-- Si la tabla no existe, crearla. Si existe, no hacer nada (idempotente)
CREATE TABLE IF NOT EXISTS public.observaciones_cpgic (
    id SERIAL PRIMARY KEY,
    propuesta_id INT NOT NULL,
    observacion TEXT NOT NULL,
    realizado_por VARCHAR(200),
    fecha_observacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_observaciones_propuestas
        FOREIGN KEY (propuesta_id)
        REFERENCES public.propuestas(id) ON DELETE CASCADE,
    CONSTRAINT chk_observacion_no_vacio
        CHECK (TRIM(observacion) <> '')
);

-- =========================================
-- CREAR ÍNDICES PARA PERFORMANCE
-- =========================================
-- Índice para búsquedas frecuentes por propuesta_id
CREATE INDEX IF NOT EXISTS idx_observaciones_propuesta_id 
    ON public.observaciones_cpgic(propuesta_id);

-- Índice para ordenar por fecha descendente
CREATE INDEX IF NOT EXISTS idx_observaciones_fecha 
    ON public.observaciones_cpgic(fecha_observacion DESC);

-- =========================================
-- COMENTARIOS PARA DOCUMENTACIÓN
-- =========================================
COMMENT ON TABLE public.observaciones_cpgic IS 
    'Observaciones realizadas por la CPGIC sobre propuestas TIC en revisión.
     Las observaciones se crean cuando una propuesta está en estado PENDIENTE
     y la CPGIC tiene comentarios que deben ser corregidos antes de aprobación.
     La propuesta cambia a estado OBSERVADA mientras tenga observaciones.';

COMMENT ON COLUMN public.observaciones_cpgic.propuesta_id IS 
    'FK hacia propuestas(id). Permite eliminar en cascada las observaciones
     cuando se elimina una propuesta.';

COMMENT ON COLUMN public.observaciones_cpgic.observacion IS 
    'Texto de la observación. Describe qué debe corregirse en la propuesta.';

COMMENT ON COLUMN public.observaciones_cpgic.realizado_por IS 
    'Nombre de la persona (miembro CPGIC) que realizó la observación.';

COMMENT ON COLUMN public.observaciones_cpgic.fecha_observacion IS 
    'Fecha y hora cuando se registró la observación.';

-- =========================================
-- CONFIRMACIÓN
-- =========================================
-- Script ejecutado correctamente.
-- Tabla observaciones_cpgic lista para HU04.
-- Compatible con Entity Framework Core y Neon PostgreSQL.
