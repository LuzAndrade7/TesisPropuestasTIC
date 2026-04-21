import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PropuestaService, Propuesta } from '../../services/propuesta.service';

@Component({
  selector: 'app-crear-propuesta',
  templateUrl: './crear-propuesta.component.html',
  styleUrls: ['./crear-propuesta.component.css']
})
export class CrearPropuestaComponent implements OnInit {
  propuesta: Propuesta = {
    titulo: '',
    proyecto: '',
    descripcion: '',
    objetivo: '',
    alcance: '',
    numeroParticipantes: 1,
    asignaturas: [],
    modulos: []
  };

  materias = [
    'Álgebra Lineal (MATD113)',
    'Cálculo en una Variable (MATD123)',
    'Mecánica Newtoniana (FISD134)',
    'Programación I (ICCD144)',
    'Comunicación Oral y Escrita (CSHD111)',
    'Ecuaciones Diferenciales Ordinarias (MATD213)',
    'Matemáticas Computacionales y Teoría de la Computación (ICCD224)',
    'Fundamentos de Electrónica para Computación (ICCD233)',
    'Programación II (ICCD244)',
    'Análisis Socioeconómico y Político del Ecuador (CSHD211)',
    'Probabilidad y Estadísticas Básicas (MATD223)',
    'Sistemas Operativos (ICCD323)',
    'Arquitectura de Computadoras (ICCD332)',
    'Estructura de Datos y Algoritmos I (ICCD343)',
    'Fundamentos de Redes y Conectividad (ICCD353)',
    'Ingeniería de Software y Requerimientos (ISWD414)',
    'Compiladores y Lenguajes (ICCD422)',
    'Fundamentos de Sistemas de Información (ISWD433)',
    'Estructura de Datos y Algoritmos II (ICCD442)',
    'Fundamentos de Bases de Datos (ISWD453)',
    'Gestión Organizacional (ADMD511)',
    'Diseño de Software (ISWD523)',
    'Computación Gráfica (ICCD533)',
    'Inteligencia Artificial y Aprendizaje Automático (ISWD543)',
    'Bases de Datos Distribuidas (ISWD553)',
    'Aplicaciones Web (ISWD613)',
    'Metodologías Ágiles (ISWD622)',
    'Construcción y Evolución de Software (ISWD633)',
    'Tecnologías de Seguridad (ICCD643)',
    'Calidad de Software (ISWD652)',
    'Gestión de Procesos y Calidad (ADMD611)',
    'Ingeniería Financiera (ADMD711)',
    'Aplicaciones Móviles (ISWD713)',
    'Interacción Humano Computador (ISWD723)',
    'Usabilidad y Accesibilidad (ISWD732)',
    'Business Intelligence (ISWD743)',
    'Verificación y Validación de Software (ISWD752)',
    'Automatización de Procesos (ISWD762)',
    'Aplicaciones Web Avanzadas (ISWD813)',
    'Desarrollo de Juegos Interactivos (ISWD823)',
    'Auditoría Informática (ISWD833)',
    'Profesionalismo en Informática (ICCD842)',
    'Desarrollo de Software Seguro (ISWD853)',
    'Sistemas Embebidos (ISWD913)',
    'Gestión de Proyectos de Software (ISWD922)',
    'Prácticas Laborales (PRLD105)',
    'Trabajo de Integración Curricular / Examen Complexivo (TITID201)'
  ];

  isLoading: boolean = false;
  editMode: boolean = false;
  propuestaId: number | null = null;

  constructor(
    private propuestaService: PropuestaService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.editMode = true;
        this.propuestaId = params['id'];
        this.cargarPropuesta(params['id']);
      }
    });

    if (this.propuesta.modulos.length === 0) {
      this.agregarModulo();
    }
  }

  cargarPropuesta(id: number): void {
    this.propuestaService.obtenerPropuesta(id).subscribe({
      next: (data) => {
        this.propuesta = data;
      },
      error: (err) => {
        console.error('Error cargando propuesta:', err);
      }
    });
  }

  agregarModulo(): void {
    this.propuesta.modulos.push({
      numero: this.propuesta.modulos.length + 1,
      nombre: '',
      descripcion: '',
      productos: '',
      actividades: []
    });
  }

  agregarAsignatura(materia: string): void {
    if (materia && !this.propuesta.asignaturas.includes(materia)) {
      this.propuesta.asignaturas.push(materia);
    }
  }

  onAsignaturaChange(event: any): void {
    const value = (event.target as HTMLSelectElement).value;
    this.agregarAsignatura(value);
    (event.target as HTMLSelectElement).value = '';
  }

  eliminarAsignatura(materia: string): void {
    this.propuesta.asignaturas = this.propuesta.asignaturas.filter(a => a !== materia);
  }

  guardarBorrador(): void {
    this.isLoading = true;
    const guardarFn = this.editMode
      ? this.propuestaService.actualizarPropuesta(this.propuestaId!, this.propuesta as any)
      : this.propuestaService.crearPropuesta(this.propuesta as any);

    guardarFn.subscribe({
      next: (data) => {
        this.isLoading = false;
        alert('Propuesta guardada como borrador');
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Error guardando propuesta:', err);
        alert('Error al guardar la propuesta');
      }
    });
  }

  enviarPropuesta(): void {
    this.isLoading = true;
    const guardarFn = this.editMode
      ? this.propuestaService.actualizarPropuesta(this.propuestaId!, this.propuesta as any)
      : this.propuestaService.crearPropuesta(this.propuesta as any);

    guardarFn.subscribe({
      next: (data) => {
        this.isLoading = false;
        alert('Propuesta enviada exitosamente');
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Error enviando propuesta:', err);
        alert('Error al enviar la propuesta');
      }
    });
  }

  cancelar(): void {
    if (confirm('¿Desea abandonar sin guardar?')) {
      this.router.navigate(['/dashboard']);
    }
  }
}
