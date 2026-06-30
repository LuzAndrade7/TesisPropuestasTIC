-- =========================================
-- SPRINT 1: INIT - Registro y guardado de propuestas TIC (HU01)
-- Base de datos: Neon PostgreSQL
-- Fecha: May 12, 2026
-- =========================================

-- DROP TABLES IF EXISTS (idempotente)
DROP TABLE IF EXISTS public.propuesta_asignaturas CASCADE;
DROP TABLE IF EXISTS public.propuestas CASCADE;
DROP TABLE IF EXISTS public.asignaturas CASCADE;
DROP TABLE IF EXISTS public.docentes CASCADE;

-- =========================================
-- TABLA DOCENTES
-- =========================================
CREATE TABLE public.docentes (
    id SERIAL PRIMARY KEY,
    nombres VARCHAR(100) NOT NULL,
    apellidos VARCHAR(100) NOT NULL,
    correo VARCHAR(150),
    titulo_academico VARCHAR(100),
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =========================================
-- TABLA ASIGNATURAS
-- =========================================
CREATE TABLE public.asignaturas (
    id SERIAL PRIMARY KEY,
    codigo VARCHAR(20) NOT NULL UNIQUE,
    nombre VARCHAR(150) NOT NULL,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =========================================
-- TABLA PROPUESTAS
-- =========================================
CREATE TABLE public.propuestas (
    id SERIAL PRIMARY KEY,
    nombre_proyecto VARCHAR(250) NOT NULL,
    numero_participantes INT NOT NULL CHECK (numero_participantes > 0 AND numero_participantes <= 5),
    profesor_id INT NOT NULL,
    descripcion TEXT NOT NULL,
    objetivo TEXT NOT NULL,
    alcance TEXT NOT NULL,
    estado VARCHAR(50) DEFAULT 'BORRADOR' CHECK (estado IN ('BORRADOR', 'PENDIENTE', 'OBSERVADA', 'APROBADA', 'RECHAZADA')),
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_envio_revision TIMESTAMP,
    CONSTRAINT fk_propuestas_docentes
        FOREIGN KEY (profesor_id)
        REFERENCES public.docentes(id) ON DELETE CASCADE
);

-- =========================================
-- TABLA PROPUESTA_ASIGNATURAS
-- =========================================
CREATE TABLE public.propuesta_asignaturas (
    id SERIAL PRIMARY KEY,
    propuesta_id INT NOT NULL,
    asignatura_id INT NOT NULL,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_propuesta_asignaturas_propuestas
        FOREIGN KEY (propuesta_id)
        REFERENCES public.propuestas(id) ON DELETE CASCADE,
    CONSTRAINT fk_propuesta_asignaturas_asignaturas
        FOREIGN KEY (asignatura_id)
        REFERENCES public.asignaturas(id),
    CONSTRAINT uq_propuesta_asignatura
        UNIQUE (propuesta_id, asignatura_id)
);

-- =========================================
-- INSERTAR DOCENTE DE PRUEBA
-- =========================================
INSERT INTO public.docentes (nombres, apellidos, correo, titulo_academico)
VALUES 
    ('Ana', 'López', 'ana.lopez@epn.edu.ec', 'PhD. Ingeniería de Software');

-- =========================================
-- INSERTAR ASIGNATURAS DE PRUEBA
-- =========================================
INSERT INTO public.asignaturas (codigo, nombre) VALUES
('ISWD453', 'Fundamentos de Bases de Datos'),
('ISWD523', 'Diseño de Software'),
('ISWD622', 'Metodologías Ágiles'),
('ISWD813', 'Aplicaciones Web Avanzadas'),
('ISWD652', 'Calidad de Software'),
('ISWD732', 'Usabilidad y Accesibilidad');

-- =========================================
-- CREAR ÍNDICES PARA PERFORMANCE
-- =========================================
CREATE INDEX idx_propuestas_profesor_id ON public.propuestas(profesor_id);
CREATE INDEX idx_propuestas_estado ON public.propuestas(estado);
CREATE INDEX idx_propuestas_fecha_creacion ON public.propuestas(fecha_creacion DESC);
CREATE INDEX idx_propuesta_asignaturas_propuesta_id ON public.propuesta_asignaturas(propuesta_id);

-- =========================================
-- CONFIRMACIÓN
-- =========================================
-- Script ejecutado correctamente. Base de datos lista para Sprint 1.
-- Docente: Ana López (ID: 1)
-- Asignaturas: 6 registradas
