-- Crear base de datos
CREATE DATABASE tesis_tic_db
    WITH
    ENCODING = 'UTF8'
    LC_COLLATE = 'es_ES.UTF-8'
    LC_CTYPE = 'es_ES.UTF-8';

-- Commectarse a la base de datos
\c tesis_tic_db;

-- Tabla Estados
CREATE TABLE public.estados (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion VARCHAR(500),
    fecha_creacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabla Lineas de Investigacion
CREATE TABLE public.lineas_investigacion (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(200) NOT NULL,
    descripcion VARCHAR(500),
    fecha_creacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabla Asignaturas
CREATE TABLE public.asignaturas (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(200) NOT NULL,
    codigo VARCHAR(20) NOT NULL UNIQUE,
    fecha_creacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabla Docentes
CREATE TABLE public.docentes (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    correo_institucional VARCHAR(150) NOT NULL UNIQUE,
    correo_personal VARCHAR(150),
    cedula VARCHAR(20) NOT NULL,
    numero_empleado VARCHAR(20),
    departamento VARCHAR(150),
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    fecha_creacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion TIMESTAMP
);

-- Tabla Estudiantes
CREATE TABLE public.estudiantes (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    matricula VARCHAR(20) NOT NULL UNIQUE,
    correo_institucional VARCHAR(150) NOT NULL UNIQUE,
    cedula VARCHAR(20) NOT NULL,
    fecha_creacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabla Propuestas
CREATE TABLE public.propuestas (
    id SERIAL PRIMARY KEY,
    titulo VARCHAR(300) NOT NULL,
    descripcion TEXT NOT NULL,
    objetivo TEXT NOT NULL,
    alcance TEXT NOT NULL,
    componentes_actividades_productos TEXT NOT NULL,
    docente_id INT NOT NULL REFERENCES public.docentes(id) ON DELETE CASCADE,
    estado_id INT NOT NULL REFERENCES public.estados(id) ON DELETE RESTRICT,
    linea_investigacion_id INT REFERENCES public.lineas_investigacion(id) ON DELETE SET NULL,
    observaciones VARCHAR(1000),
    numero_participantes INT NOT NULL,
    departamento VARCHAR(150) NOT NULL,
    facultad VARCHAR(200) NOT NULL,
    fecha_creacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion TIMESTAMP,
    fecha_envio_primera TIMESTAMP
);

-- Tabla de Relación Propuestas-Estudiantes
CREATE TABLE public.propuestas_estudiantes (
    id SERIAL PRIMARY KEY,
    propuesta_id INT NOT NULL REFERENCES public.propuestas(id) ON DELETE CASCADE,
    estudiante_id INT NOT NULL REFERENCES public.estudiantes(id) ON DELETE CASCADE,
    fecha_asignacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(propuesta_id, estudiante_id)
);

-- Tabla de Relación Propuestas-Asignaturas
CREATE TABLE public.propuestas_asignaturas (
    propuesta_id INT NOT NULL REFERENCES public.propuestas(id) ON DELETE CASCADE,
    asignatura_id INT NOT NULL REFERENCES public.asignaturas(id) ON DELETE CASCADE,
    PRIMARY KEY (propuesta_id, asignatura_id)
);

-- Crear índices
CREATE INDEX idx_propuestas_docente_id ON public.propuestas(docente_id);
CREATE INDEX idx_propuestas_estado_id ON public.propuestas(estado_id);
CREATE INDEX idx_propuestas_linea_investigacion_id ON public.propuestas(linea_investigacion_id);
CREATE INDEX idx_propuestas_estudiantes_propuesta_id ON public.propuestas_estudiantes(propuesta_id);
CREATE INDEX idx_propuestas_estudiantes_estudiante_id ON public.propuestas_estudiantes(estudiante_id);
CREATE INDEX idx_docentes_activo ON public.docentes(activo);

-- Insertar Estados predeterminados
INSERT INTO public.estados (nombre, descripcion) VALUES
('Pendiente', 'Propuesta creada, pendiente de revisión'),
('Enviada', 'Propuesta enviada para revisión por CPGIC'),
('En Revisión', 'Propuesta bajo revisión'),
('Aprobada', 'Propuesta aprobada'),
('Rechazada', 'Propuesta rechazada'),
('Observada', 'Propuesta con observaciones, requiere ajustes');

-- Insertar Líneas de Investigación de ejemplo
INSERT INTO public.lineas_investigacion (nombre, descripcion) VALUES
('Ingeniería de Software', 'Línea de investigación enfocada en el desarrollo y evolución de software'),
('Seguridad Informática', 'Línea de investigación en seguridad de sistemas y aplicaciones'),
('Inteligencia Artificial', 'Línea de investigación en IA y machine learning'),
('Bases de Datos', 'Línea de investigación en sistemas de gestión de bases de datos'),
('Cloud Computing', 'Línea de investigación en computación en la nube');

-- Insertar Asignaturas de ejemplo
INSERT INTO public.asignaturas (nombre, codigo) VALUES
('Ingeniería de Software I', 'IN-501'),
('Ingeniería de Software II', 'IN-502'),
('Tecnologías Web', 'IN-503'),
('Bases de Datos', 'IN-504'),
('Programación Avanzada', 'IN-505');
