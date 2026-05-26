import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PropuestaService } from '../../services/propuesta.service';
import { Asignatura, Docente, CreatePropuestaRequest } from '../../models/propuesta.model';

/**
 * T02: Componente Formulario de Propuesta
 * Ubicación: src/app/components/formulario-propuesta/formulario-propuesta.component.ts
 * 
 * Responsabilidades:
 * - Mostrar formulario reactivo para crear/editar propuestas
 * - Validar entrada de datos
 * - Guardar como borrador (T01)
 * - Enviar a revisión (cambiar estado a PENDIENTE) - HU03 T08
 * - Integración con PropuestaService (T03)
 * - Deshabilitar edición si estado es PENDIENTE (HU03 T09)
 */
@Component({
  selector: 'app-formulario-propuesta',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './formulario-propuesta.component.html',
  styleUrls: ['./formulario-propuesta.component.scss']
})
export class FormularioPropuestaComponent implements OnInit {
  // Formulario reactivo
  formulario: FormGroup;

  // Datos para dropdowns
  asignaturas: Asignatura[] = [];
  docentes: Docente[] = [];

  // Estado de la interfaz
  cargando = false;
  guardando = false;
  enviando = false;
  mensajeError = '';
  mensajeExito = '';
  mostrarModalConfirmacion = false;

  // Propuesta actual (si estamos editando)
  propuestaId: number | null = null;
  estadoPropuesta: string = 'BORRADOR'; // HU03 T09: Para deshabilitar campos si está PENDIENTE
  puedeEditar = true; // HU05 T15: Indica si se puede editar (BORRADOR u OBSERVADA)
  noEditable = false; // HU05 T15: Indica si NO se puede editar

  // HU04 T11-T13: Observaciones CPGIC
  observaciones: any[] = []; // Lista de observaciones
  tieneObservaciones = false; // Indica si hay observaciones
  reEnviando = false; // Estado de reenvío
  mostrarModalReenvio = false; // Modal de confirmación para reenvío

  // Validaciones
  readonly MAX_PARTICIPANTES = 5;
  readonly MIN_NOMBRES_PROYECTO = 10;
  readonly MAX_NOMBRES_PROYECTO = 250;
  readonly MIN_DESCRIPCION = 20;
  readonly MIN_OBJETIVO = 20;
  readonly MIN_ALCANCE = 20;

  constructor(
    private formBuilder: FormBuilder,
    private propuestaService: PropuestaService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    // Inicializar formulario con validaciones
    this.formulario = this.formBuilder.group({
      nombreProyecto: [
        '',
        [
          Validators.required,
          Validators.minLength(this.MIN_NOMBRES_PROYECTO),
          Validators.maxLength(this.MAX_NOMBRES_PROYECTO)
        ]
      ],
      numeroParticipantes: [
        1,
        [
          Validators.required,
          Validators.min(1),
          Validators.max(this.MAX_PARTICIPANTES)
        ]
      ],
      profesorId: [
        1,
        [Validators.required]
      ],
      descripcion: [
        '',
        [
          Validators.required,
          Validators.minLength(20)
        ]
      ],
      objetivo: [
        '',
        [
          Validators.required,
          Validators.minLength(20)
        ]
      ],
      alcance: [
        '',
        [
          Validators.required,
          Validators.minLength(20)
        ]
      ],
      asignaturaIds: [
        [],
        [Validators.required]
      ]
    });
  }

  ngOnInit(): void {
    console.log('✅ T02: Iniciando componente FormularioPropuestaComponent');
    
    // Cargar datos iniciales
    this.cargarAsignaturas();
    this.cargarDocentes();

    // Si hay ID en la ruta, cargar propuesta existente
    this.propuestaId = this.route.snapshot.params['id'] || null;
    if (this.propuestaId) {
      this.cargarPropuesta(this.propuestaId);
    }
  }

  /**
   * T02: Cargar lista de asignaturas para dropdown
   */
  cargarAsignaturas(): void {
    this.cargando = true;
    this.propuestaService.obtenerAsignaturas().subscribe({
      next: (asignaturas) => {
        this.asignaturas = asignaturas;
        console.log('✅ T02: Asignaturas cargadas:', asignaturas.length);
      },
      error: (error) => {
        console.error('❌ T02: Error cargando asignaturas:', error);
        this.mensajeError = 'Error al cargar asignaturas';
      },
      complete: () => {
        this.cargando = false;
      }
    });
  }

  /**
   * T02: Cargar lista de docentes
   */
  cargarDocentes(): void {
    this.propuestaService.obtenerDocentes().subscribe({
      next: (docentes) => {
        this.docentes = docentes;
        console.log('✅ T02: Docentes cargados:', docentes.length);
      },
      error: (error) => {
        console.error('❌ T02: Error cargando docentes:', error);
        this.mensajeError = 'Error al cargar docentes';
      }
    });
  }

  /**
   * T02: Cargar propuesta existente para edición
   * HU05 T15: Permite edición en estado BORRADOR u OBSERVADA
   */
  cargarPropuesta(id: number): void {
    this.cargando = true;
    this.propuestaService.obtenerPropuestaById(id).subscribe({
      next: (propuesta) => {
        console.log('✅ T02: Propuesta cargada:', propuesta);
        
        // Guardar estado
        this.estadoPropuesta = propuesta.estado;
        
        // HU05 T15: Permitir edición solo en BORRADOR u OBSERVADA
        const estadosEditables = ['BORRADOR', 'OBSERVADA'];
        this.puedeEditar = estadosEditables.includes(propuesta.estado);
        this.noEditable = !this.puedeEditar;
        
        // Llenar formulario con datos de la propuesta
        this.formulario.patchValue({
          nombreProyecto: propuesta.nombreProyecto,
          numeroParticipantes: propuesta.numeroParticipantes,
          profesorId: propuesta.profesorId,
          descripcion: propuesta.descripcion,
          objetivo: propuesta.objetivo,
          alcance: propuesta.alcance,
          asignaturaIds: propuesta.asignaturas.map(a => a.id)
        });

        // HU05 T15: Deshabilitar campos si NO se puede editar
        if (!this.puedeEditar) {
          this.formulario.disable();
          this.mensajeError = `❌ No se puede editar una propuesta en estado '${propuesta.estado}'. ` +
                             `Solo se pueden editar propuestas en estado BORRADOR u OBSERVADA.`;
        }

        // HU04 T11: Cargar observaciones si propuesta está OBSERVADA
        if (propuesta.estado === 'OBSERVADA') {
          this.cargarObservaciones(id);
        }
      },
      error: (error) => {
        console.error('❌ T02: Error cargando propuesta:', error);
        this.mensajeError = 'Error al cargar la propuesta';
      },
      complete: () => {
        this.cargando = false;
      }
    });
  }

  /**
   * HU04 T11: Cargar observaciones CPGIC para propuesta OBSERVADA
   */
  cargarObservaciones(propuestaId: number): void {
    this.propuestaService.obtenerObservaciones(propuestaId).subscribe({
      next: (observaciones) => {
        this.observaciones = observaciones;
        this.tieneObservaciones = observaciones.length > 0;
        console.log('✅ HU04 T11: Observaciones cargadas:', observaciones.length);
      },
      error: (error) => {
        console.error('❌ HU04 T11: Error cargando observaciones:', error);
        // No mostrar error si no hay observaciones, es normal
        this.observaciones = [];
        this.tieneObservaciones = false;
      }
    });
  }

  /**
   * HU04 T12: Reenviar propuesta después de correcciones
   * Limpia observaciones y cambia estado OBSERVADA → PENDIENTE
   */
  reenviarPropuesta(): void {
    if (!this.propuestaId) {
      this.mensajeError = 'Error: ID de propuesta no disponible';
      return;
    }

    // Mostrar modal de confirmación
    this.mostrarModalReenvio = true;
  }

  /**
   * HU04 T12: Confirmar reenvío
   */
  confirmarReenvio(): void {
    this.mostrarModalReenvio = false;

    if (!this.propuestaId) {
      this.mensajeError = 'Error: ID de propuesta no disponible';
      return;
    }

    this.reEnviando = true;
    this.mensajeError = '';

    console.log('✅ HU04 T12: Reenviando propuesta', this.propuestaId);

    this.propuestaService.reenviarDespuesDeObservaciones(this.propuestaId).subscribe({
      next: (propuesta) => {
        console.log('✅ HU04 T12: Propuesta reenviada:', propuesta);
        this.mensajeExito = '✓ Propuesta reenviada a revisión exitosamente';
        this.estadoPropuesta = propuesta.estado;
        
        // Limpiar observaciones y deshabilitar formulario
        this.observaciones = [];
        this.tieneObservaciones = false;
        this.formulario.disable();
        
        // Redirigir al dashboard después de 1.5 segundos
        setTimeout(() => {
          this.router.navigate(['/tablero']);
        }, 1500);
      },
      error: (error) => {
        console.error('❌ HU04 T12: Error reenviando:', error);
        this.mensajeError = error.message || 'Error al reenviar la propuesta';
      },
      complete: () => {
        this.reEnviando = false;
      }
    });
  }

  /**
   * HU04 T12: Cancelar modal de reenvío
   */
  cancelarReenvio(): void {
    this.mostrarModalReenvio = false;
  }

  /**
   * T01: Guardar como borrador
   * HU05 T16: Guardar cambios de propuesta editada
   * - NO requiere validación - permite guardar incompleto
   * - Estado: BORRADOR
   * - Permite guardar incompleto (puede estar vacío)
   */
  guardarBorrador(): void {
    // SIN VALIDACIÓN - permitir guardar incompleto en estado BORRADOR
    this.guardando = true;
    this.mensajeError = '';

    const datos = this.construirPropuesta();
    
    console.log('✅ T01/HU05: Guardando como borrador:', datos);

    const operacion = this.propuestaId 
      ? this.propuestaService.actualizarPropuesta(this.propuestaId, datos)
      : this.propuestaService.guardarBorrador(datos as CreatePropuestaRequest);

    operacion.subscribe({
      next: (propuesta) => {
        console.log('✅ T01/HU05: Borrador/Propuesta guardado exitosamente:', propuesta);
        this.mensajeExito = this.propuestaId 
          ? '✓ Cambios guardados exitosamente'
          : '✓ Propuesta guardada como borrador';
        
        // Si fue creación, actualizar el ID
        if (!this.propuestaId) {
          this.propuestaId = propuesta.id;
        }

        // Redirigir al dashboard después de 1.5 segundos
        setTimeout(() => {
          this.router.navigate(['/tablero']);
        }, 1500);
      },
      error: (error) => {
        console.error('❌ T01/HU05: Error guardando:', error);
        this.mensajeError = error.message || 'Error al guardar la propuesta';
      },
      complete: () => {
        this.guardando = false;
      }
    });
  }

  /**
   * HU03 T08: Enviar propuesta a revisión
   * - Valida que el formulario esté completo
   * - Muestra modal de confirmación
   * - Llama al nuevo endpoint POST /api/propuestas/{id}/enviar-revision
   * - El backend valida completamente antes de cambiar estado
   */
  enviarPropuesta(): void {
    // Validación frontend
    if (this.formulario.invalid) {
      this.mostrarErroresValidacion();
      return;
    }

    // Mostrar modal de confirmación
    this.mostrarModalConfirmacion = true;
  }

  /**
   * HU03 T09: Confirmar envío a revisión
   * Luego de confirmar en el modal
   */
  confirmarEnvioRevision(): void {
    this.mostrarModalConfirmacion = false;

    // Primero guardar si no existe el ID
    if (!this.propuestaId) {
      const datos = this.construirPropuesta();
      this.propuestaService.crearPropuesta(datos as CreatePropuestaRequest).subscribe({
        next: (propuesta) => {
          this.propuestaId = propuesta.id;
          this.enviarARevision(propuesta.id);
        },
        error: (error) => {
          console.error('❌ HU03: Error creando propuesta:', error);
          this.mensajeError = error.message || 'Error al crear la propuesta';
        }
      });
    } else {
      this.enviarARevision(this.propuestaId);
    }
  }

  /**
   * HU03 T07: Llamar al nuevo endpoint de envío a revisión
   */
  private enviarARevision(propuestaId: number): void {
    this.enviando = true;
    this.mensajeError = '';

    console.log('✅ HU03: Enviando propuesta', propuestaId, 'a revisión');

    this.propuestaService.enviarARevision(propuestaId).subscribe({
      next: (propuesta) => {
        console.log('✅ HU03: Propuesta enviada a revisión:', propuesta);
        this.mensajeExito = '✓ Propuesta enviada a revisión exitosamente';
        this.estadoPropuesta = propuesta.estado;
        
        // Deshabilitar campos después del envío
        this.formulario.disable();
        
        // Redirigir al dashboard después de 1.5 segundos
        setTimeout(() => {
          this.router.navigate(['/tablero']);
        }, 1500);
      },
      error: (error) => {
        console.error('❌ HU03: Error enviando a revisión:', error);
        this.mensajeError = error.message || 'Error al enviar la propuesta a revisión';
        
        // Mostrar errores específicos del backend
        if (error.message) {
          this.mensajeError = error.message;
        }
      },
      complete: () => {
        this.enviando = false;
      }
    });
  }

  /**
   * HU03 T09: Mostrar errores de validación específicos
   */
  private mostrarErroresValidacion(): void {
    const errores: string[] = [];

    const campos = ['nombreProyecto', 'numeroParticipantes', 'descripcion', 'objetivo', 'alcance', 'asignaturaIds', 'profesorId'];
    
    campos.forEach(campo => {
      const control = this.formulario.get(campo);
      if (control && control.invalid && control.touched) {
        const error = this.obtenerErrorCampo(campo);
        if (error) {
          errores.push(error);
        }
      }
    });

    if (errores.length > 0) {
      this.mensajeError = 'Por favor completa los siguientes campos:\n' + errores.join('\n');
    } else {
      this.mensajeError = 'Por favor completa todos los campos requeridos correctamente';
    }
  }

  /**
   * Cancelar modal de confirmación
   */
  cancelarEnvio(): void {
    this.mostrarModalConfirmacion = false;
  }

  /**
   * Cancelar edición y volver atrás
   */
  cancelar(): void {
    this.router.navigate(['/']);
  }

  /**
   * Construir objeto propuesta desde formulario
   */
  private construirPropuesta(): CreatePropuestaRequest | Partial<CreatePropuestaRequest> {
    const valores = this.formulario.value;
    
    return {
      nombreProyecto: valores.nombreProyecto.trim(),
      numeroParticipantes: parseInt(valores.numeroParticipantes, 10),
      profesorId: parseInt(valores.profesorId, 10),
      descripcion: valores.descripcion.trim(),
      objetivo: valores.objetivo.trim(),
      alcance: valores.alcance.trim(),
      asignaturaIds: valores.asignaturaIds || []
    };
  }

  /**
   * Obtener mensaje de error para campo específico
   */
  obtenerErrorCampo(nombreCampo: string): string {
    const campo = this.formulario.get(nombreCampo);

    if (!campo || !campo.errors || !campo.touched) {
      return '';
    }

    if (campo.errors['required']) {
      return `${this.obtenerLabelCampo(nombreCampo)} es requerido`;
    }
    if (campo.errors['minlength']) {
      return `${this.obtenerLabelCampo(nombreCampo)} debe tener mínimo ${campo.errors['minlength'].requiredLength} caracteres`;
    }
    if (campo.errors['maxlength']) {
      return `${this.obtenerLabelCampo(nombreCampo)} debe tener máximo ${campo.errors['maxlength'].requiredLength} caracteres`;
    }
    if (campo.errors['min']) {
      return `${this.obtenerLabelCampo(nombreCampo)} debe ser mínimo ${campo.errors['min'].min}`;
    }
    if (campo.errors['max']) {
      return `${this.obtenerLabelCampo(nombreCampo)} debe ser máximo ${campo.errors['max'].max}`;
    }

    return 'Error en este campo';
  }

  /**
   * Obtener etiqueta legible del campo
   */
  private obtenerLabelCampo(nombreCampo: string): string {
    const labels: { [key: string]: string } = {
      nombreProyecto: 'Nombre del proyecto',
      numeroParticipantes: 'Número de participantes',
      profesorId: 'Profesor',
      descripcion: 'Descripción',
      objetivo: 'Objetivo',
      alcance: 'Alcance',
      asignaturaIds: 'Asignaturas'
    };

    return labels[nombreCampo] || nombreCampo;
  }

  /**
   * Verificar si campo es inválido y ha sido tocado
   */
  campoInvalido(nombreCampo: string): boolean {
    const campo = this.formulario.get(nombreCampo);
    return !!(campo && campo.invalid && campo.touched);
  }

  /**
   * Verificar si es nuevo (creando vs editando)
   */
  esNuevo(): boolean {
    return !this.propuestaId;
  }

  /**
   * T02: Manejar cambio de checkbox de asignatura
   */
  toggleAsignatura(asignaturaId: number, isChecked: boolean): void {
    const control = this.formulario.get('asignaturaIds');
    if (!control) return;

    const currentValue = control.value || [];
    let newValue: number[];

    if (isChecked) {
      // Agregar si no está
      newValue = currentValue.includes(asignaturaId) 
        ? currentValue 
        : [...currentValue, asignaturaId];
    } else {
      // Remover si está
      newValue = currentValue.filter((id: number) => id !== asignaturaId);
    }

    control.setValue(newValue);
  }
}
