import { Component, OnInit, OnDestroy, Output, EventEmitter, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
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

  // Estado de la interfaz
  cargando = false;
  buscando = false;
  guardando = false;
  mostrandoBusqueda = false;
  mensajeError = '';
  mensajeExito = '';
  mostrarAdvertencia = false; // Advertencia si hay cambios desde APROBADA

  // Control del término de búsqueda
  private busqueda$ = new Subject<string>();
  private destroy$ = new Subject<void>();

  // Límites
  MAX_ESTUDIANTES = 5;

  constructor(
    private formBuilder: FormBuilder,
    private propuestaService: PropuestaService
  ) {
    this.formulario = this.formBuilder.group({
      busqueda: [''],
      motivo: ['', Validators.required]
    });
  }

  ngOnInit(): void {
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

    this.propuestaService.obtenerTodosEstudiantes()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (estudiantes: any[]) => {
          this.todosEstudiantes = estudiantes;
          this.estudiantesFiltrados = estudiantes;
          this.cargando = false;

          // Pre-seleccionar estudiantes actuales
          if (this.estudiantesActuales.length > 0) {
            this.estudiantesSeleccionados = [...this.estudiantesActuales];
          }
        },
        error: (error: any) => {
          this.mensajeError = 'Error al cargar estudiantes';
          this.cargando = false;
          console.error('Error cargando estudiantes:', error);
        }
      });
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

    if (this.estudiantesSeleccionados.length === 0) {
      this.mensajeError = 'Debes seleccionar al menos un estudiante';
      return false;
    }

    if (this.mostrarAdvertencia && !this.formulario.get('motivo')?.value?.trim()) {
      this.mensajeError = 'Debes proporcionar un motivo para los cambios';
      return false;
    }

    return true;
  }

  /**
   * Guarda la asignación de estudiantes
   */
  guardar(): void {
    if (!this.validar()) {
      return;
    }

    this.guardando = true;
    const estudianteIds = this.estudiantesSeleccionados.map(e => e.id);
    const motivo = this.formulario.get('motivo')?.value || 
                   'Asignación inicial de estudiantes';

    this.propuestaService.asignarEstudiantes(this.propuestaId!, estudianteIds, motivo)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (resultado: any) => {
          this.mensajeExito = 'Estudiantes asignados correctamente';
          this.guardando = false;

          // Emitir evento de completación
          setTimeout(() => {
            this.asignacionCompleta.emit({
              estudianteIds: estudianteIds,
              motivo: motivo,
              cambioEstado: this.mostrarAdvertencia
            });
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
    return `${estudiante.apellidos} ${estudiante.nombres}`;
  }
}
