import { Component, OnInit, OnDestroy, Output, EventEmitter, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { PropuestaService } from '../../services/propuesta.service';

/**
 * HU07 T21: Componente modal para asignar estudiantes a propuestas
 * Ubicación: src/app/components/asignar-estudiantes/asignar-estudiantes.component.ts
 *
 * Responsabilidades:
 * - Mostrar modal para asignación de estudiantes
 * - Búsqueda de estudiantes (autocomplete)
 * - Validación: máximo 5 estudiantes
 * - Mostrar advertencia si cambian estudiantes desde APROBADA
 * - Solicitar motivo de cambio
 */
@Component({
  selector: 'app-asignar-estudiantes',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './asignar-estudiantes.component.html',
  styleUrls: ['./asignar-estudiantes.component.scss']
})
export class AsignarEstudiantesComponent implements OnInit, OnDestroy {
  @Input() propuestaId: number | null = null;
  @Input() estadoPropuesta: string = 'PENDIENTE'; // APROBADA o PENDIENTE
  @Input() estudiantesActuales: any[] = []; // Estudiantes ya asignados
  @Output() asignacionCompleta = new EventEmitter<any>();
  @Output() cerrar = new EventEmitter<void>();

  // Formulario reactivo
  formulario: FormGroup;

  // Datos para el dropdown y búsqueda
  todosEstudiantes: any[] = [];
  estudiantesFiltrados: any[] = [];
  estudiantesSeleccionados: any[] = [];
  detalle: any = null;
  estudiantesPorModulo: { [orden: number]: string } = {};

  // Estado de la interfaz
  cargando = false;
  buscando = false;
  guardando = false;
  mostrandoBusqueda = false;
  mensajeError = '';
  mensajeExito = '';
  mostrarAdvertencia = false; // Advertencia si hay cambios desde APROBADA
  returnUrl = '';
  intentoGuardar = false;

  // Control del término de búsqueda
  private busqueda$ = new Subject<string>();
  private destroy$ = new Subject<void>();

  // Límites
  MAX_ESTUDIANTES = 5;

  constructor(
    private formBuilder: FormBuilder,
    private propuestaService: PropuestaService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.formulario = this.formBuilder.group({
      busqueda: [''],
      motivo: ['']
    });
  }

  ngOnInit(): void {
    const idRuta = Number(this.route.snapshot.paramMap.get('id'));
    if (!this.propuestaId && idRuta) {
      this.propuestaId = idRuta;
    }

    this.returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') || '';

    if (this.propuestaId) {
      this.cargarDetallePropuesta(this.propuestaId);
      this.cargarEstudiantesAsignados(this.propuestaId);
    }

    // Cargar estudiantes disponibles
    this.cargarEstudiantes();

    // Setup búsqueda con debounce
    this.busqueda$
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntil(this.destroy$)
      )
      .subscribe((termino: string) => {
        this.buscarEstudiantes(termino);
      });

    // Escuchar cambios en el campo de búsqueda
    this.formulario.get('busqueda')?.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe((valor: string) => {
        this.busqueda$.next(valor);
      });

    // Mostrar advertencia si ya hay estudiantes asignados y estado es APROBADA
    if (this.estudiantesActuales.length > 0 && this.estadoPropuesta === 'APROBADA') {
      this.mostrarAdvertencia = true;
    }
  }

  /**
   * Carga el detalle de la propuesta para mostrar nombre, estado y módulos.
   */
  private cargarDetallePropuesta(propuestaId: number): void {
    this.propuestaService.obtenerDetalleCompleto(propuestaId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (detalle: any) => {
          this.detalle = detalle;
          this.estadoPropuesta = detalle?.estado || this.estadoPropuesta;
          this.inicializarSeleccionPorModulo();
        },
        error: (error: any) => {
          this.mensajeError = error?.message || 'Error al cargar la propuesta';
        }
      });
  }

  /**
   * Carga estudiantes ya asignados cuando la pantalla se abre por ruta.
   */
  private cargarEstudiantesAsignados(propuestaId: number): void {
    this.propuestaService.obtenerEstudiantesAsignados(propuestaId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (estudiantes: any[]) => {
          this.estudiantesActuales = estudiantes;
          this.mostrarAdvertencia = estudiantes.length > 0 && this.estadoPropuesta === 'APROBADA';
          this.estudiantesSeleccionados = estudiantes
            .map(asignacion => asignacion?.estudiante || asignacion)
            .filter(Boolean);
          this.inicializarSeleccionPorModulo();
        },
        error: () => {
          this.estudiantesActuales = [];
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Carga todos los estudiantes disponibles
   */
  private cargarEstudiantes(): void {
    this.cargando = true;
    this.mensajeError = '';

    this.cargando = false;
  }

  /**
   * Busca estudiantes por término (nombre, apellido, correo)
   */
  private buscarEstudiantes(termino: string): void {
    if (!termino.trim()) {
      this.estudiantesFiltrados = this.todosEstudiantes;
      return;
    }

    this.buscando = true;
    this.propuestaService.buscarEstudiantes(termino)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (resultados: any[]) => {
          this.estudiantesFiltrados = resultados;
          this.buscando = false;
        },
        error: (error: any) => {
          console.error('Error buscando estudiantes:', error);
          this.buscando = false;
        }
      });
  }

  /**
   * Agrega un estudiante a la lista de selección
   */
  agregarEstudiante(estudiante: any): void {
    // Validar que no esté ya seleccionado
    if (this.estudiantesSeleccionados.some(e => e.id === estudiante.id)) {
      this.mensajeError = 'Este estudiante ya está seleccionado';
      return;
    }

    // Validar límite de 5 estudiantes
    if (this.estudiantesSeleccionados.length >= this.MAX_ESTUDIANTES) {
      this.mensajeError = `No se pueden seleccionar más de ${this.MAX_ESTUDIANTES} estudiantes`;
      return;
    }

    // Agregar estudiante
    this.estudiantesSeleccionados.push(estudiante);
    this.mensajeError = '';
    this.formulario.get('busqueda')?.setValue('');
    this.mostrandoBusqueda = false;
  }

  /**
   * Remueve un estudiante de la lista de selección
   */
  removerEstudiante(estudianteId: number): void {
    this.estudiantesSeleccionados = this.estudiantesSeleccionados.filter(e => e.id !== estudianteId);
  }

  /**
   * Verifica si un estudiante está seleccionado
   */
  estaSeleccionado(estudianteId: number): boolean {
    return this.estudiantesSeleccionados.some(e => e.id === estudianteId);
  }

  /**
   * Muestra/oculta la lista de búsqueda
   */
  toggleBusqueda(): void {
    this.mostrandoBusqueda = !this.mostrandoBusqueda;
  }

  /**
   * Cierra la búsqueda
   */
  cerrarBusqueda(): void {
    this.mostrandoBusqueda = false;
  }

  /**
   * Valida el formulario antes de guardar
   */
  private validar(): boolean {
    this.mensajeError = '';

    if (!this.propuestaId || this.propuestaId <= 0) {
      this.mensajeError = 'ID de propuesta inválido';
      return false;
    }

    const participantes = Number(this.detalle?.numeroParticipantes || 0);

    if (participantes < 2 || participantes > 5) {
      this.mensajeError = 'La propuesta debe tener entre 2 y 5 participantes.';
      return false;
    }

    if (this.modulosVisuales.length !== participantes) {
      this.mensajeError = `La propuesta debe tener exactamente ${participantes} módulos, uno por participante.`;
      return false;
    }

    if (this.estudiantesSeleccionados.length !== participantes) {
      this.mensajeError = `Debe asignar exactamente ${participantes} estudiantes, uno por módulo.`;
      return false;
    }

    return true;
  }

  /**
   * Guarda la asignación de estudiantes
   */
  guardar(): void {
    this.intentoGuardar = true;
    this.sincronizarSeleccionDesdeModulos();

    if (!this.validar()) {
      setTimeout(() => {
        document.querySelector('.asignar-estudiantes__alerta--error, .asignar-estudiantes__modulo--invalido')
          ?.scrollIntoView({ behavior: 'smooth', block: 'center' });
      });
      return;
    }

    this.guardando = true;
    const nombresEstudiante = this.estudiantesSeleccionados.map(e => this.getNombreCompleto(e));
    const motivo = this.formulario.get('motivo')?.value?.trim() ||
                   'Asignacion o actualizacion de estudiantes en una propuesta ya aprobada';

    this.propuestaService.asignarEstudiantesPorNombre(this.propuestaId!, nombresEstudiante, motivo)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.mensajeExito = this.estadoPropuesta === 'APROBADA'
            ? 'Estudiantes asignados correctamente. La propuesta fue reenviada a revision.'
            : 'Estudiantes asignados correctamente';
          this.guardando = false;

          setTimeout(() => {
            const evento = {
              nombresEstudiante,
              motivo,
              cambioEstado: this.estadoPropuesta === 'APROBADA'
            };
            this.asignacionCompleta.emit(evento);
            this.navegarRetorno();
          }, 1000);
        },
        error: (error: any) => {
          this.mensajeError = error?.message || 'Error al asignar estudiantes';
          this.guardando = false;
          console.error('Error asignando estudiantes:', error);
        }
      });
  }

  /**
   * Cierra el modal sin guardar
   */
  cerrarModal(): void {
    if (this.returnUrl) {
      this.navegarRetorno();
      return;
    }

    if (this.propuestaId) {
      this.router.navigate(['/propuestas', this.propuestaId, 'detalle']);
      return;
    }

    this.cerrar.emit();
  }

  /**
   * Obtiene el número de estudiantes seleccionados
   */
  get totalSeleccionados(): number {
    return this.estudiantesSeleccionados.length;
  }

  /**
   * Verifica si alcanzó el máximo de estudiantes
   */
  get alcanzadoMaximo(): boolean {
    return this.totalSeleccionados >= this.MAX_ESTUDIANTES;
  }

  /**
   * Obtiene el nombre completo de un estudiante
   */
  getNombreCompleto(estudiante: any): string {
    const dato = estudiante?.estudiante || estudiante;
    return dato?.nombresEstudiante ||
      `${dato?.nombres || ''} ${dato?.apellidos || ''}`.trim() ||
      dato?.nombreCompleto ||
      'Estudiante';
  }

  /**
   * Obtiene el nombre de un estudiante por identificador.
   */
  nombreEstudiantePorId(nombreEstudiante: string): string {
    return this.texto(nombreEstudiante?.trim() || 'Sin estudiante');
  }

  /**
   * Regresa al origen desde donde se abrió la pantalla.
   */
  private navegarRetorno(): void {
    if (this.returnUrl) {
      this.router.navigateByUrl(this.returnUrl);
      return;
    }

    this.router.navigate(['/propuestas', this.propuestaId, 'detalle']);
  }

  /**
   * Devuelve los módulos reales o módulos visuales cuando la propuesta aún no los persiste.
   */
  get modulosVisuales(): any[] {
    if (this.detalle?.componentes?.length) {
      return this.detalle.componentes;
    }

    return [];
  }

  /**
   * Cuenta módulos sin estudiante seleccionado.
   */
  get modulosSinEstudiante(): number {
    return this.modulosVisuales.filter(modulo => !this.estudiantesPorModulo[modulo.orden]).length;
  }

  /**
   * Cambia el estudiante asignado visualmente a un módulo.
   */
  seleccionarEstudianteModulo(orden: number, nombreEstudiante: string): void {
    this.estudiantesPorModulo[orden] = nombreEstudiante;
    this.sincronizarSeleccionDesdeModulos();
    this.mensajeError = '';
  }

  /**
   * Inicializa la relación visual módulo-estudiante a partir de lo que ya existe.
   */
  private inicializarSeleccionPorModulo(): void {
    this.modulosVisuales.forEach((modulo, index) => {
      const estudiante = modulo?.estudiante || this.estudiantesSeleccionados[index] || null;
      this.estudiantesPorModulo[modulo.orden] = estudiante ? this.getNombreCompleto(estudiante) : '';
    });

    this.sincronizarSeleccionDesdeModulos();
  }

  /**
   * Convierte las selecciones por módulo en la lista real que usa el backend.
   */
  private sincronizarSeleccionDesdeModulos(): void {
    const nombres = Object.values(this.estudiantesPorModulo)
      .map(nombre => nombre.trim())
      .filter(nombre => nombre.length > 0);
    const nombresUnicos = Array.from(new Set(nombres.map(nombre => nombre.toLocaleLowerCase())))
      .map(nombreNormalizado => nombres.find(nombre => nombre.toLocaleLowerCase() === nombreNormalizado) || '');

    this.estudiantesSeleccionados = nombresUnicos
      .filter(Boolean)
      .map((nombre, index) => ({
        id: index + 1,
        nombresEstudiante: nombre
      }));
  }

  /**
   * Texto de resolución para el resumen de propuesta.
   */
  get resolucionResumen(): string {
    return this.texto(this.detalle?.resolucionCpgic || 'Res. 0042-CPGIC-2026');
  }

  /**
   * Corrige texto que llega con codificación rota desde datos existentes.
   */
  texto(valor: any): string {
    if (valor === null || valor === undefined) {
      return '';
    }

    return String(valor)
      .replace(/Ã¡/g, 'á')
      .replace(/Ã©/g, 'é')
      .replace(/Ã­/g, 'í')
      .replace(/Ã³/g, 'ó')
      .replace(/Ãº/g, 'ú')
      .replace(/Ã±/g, 'ñ')
      .replace(/\u00C3\u0081/g, 'Á')
      .replace(/Ã‰/g, 'É')
      .replace(/\u00C3\u008D/g, 'Í')
      .replace(/Ã“/g, 'Ó')
      .replace(/Ãš/g, 'Ú')
      .replace(/Ã‘/g, 'Ñ')
      .replace(/Â¿/g, '¿')
      .replace(/Â¡/g, '¡')
      .replace(/Â°/g, '°')
      .replace(/Â/g, '')
      .replace(/â€”/g, '—')
      .replace(/â€“/g, '–')
      .replace(/â€˜/g, '‘')
      .replace(/â€™/g, '’')
      .replace(/â€œ/g, '“')
      .replace(/â€/g, '”');
  }

}
