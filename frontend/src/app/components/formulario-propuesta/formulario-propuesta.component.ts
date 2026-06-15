import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PropuestaService } from '../../services/propuesta.service';
import { Asignatura, Docente, CreatePropuestaRequest } from '../../models/propuesta.model';

interface ActividadFormulario {
  nombre: string;
  horas: number;
}

interface ModuloFormulario {
  nombre: string;
  descripcion: string;
  productos: string;
  estudiante: string;
  actividades: ActividadFormulario[];
}

type CampoFormato = 'negrita' | 'cursiva' | 'numerada' | 'vineta';

/**
 * T02: Componente Formulario de Propuesta
 * UbicaciÃ³n: src/app/components/formulario-propuesta/formulario-propuesta.component.ts
 * 
 * Responsabilidades:
 * - Mostrar formulario reactivo para crear/editar propuestas
 * - Validar entrada de datos
 * - Guardar como borrador (T01)
 * - Enviar a revisiÃ³n (cambiar estado a PENDIENTE) - HU03 T08
 * - IntegraciÃ³n con PropuestaService (T03)
 * - Deshabilitar ediciÃ³n si estado es PENDIENTE (HU03 T09)
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
  menuAsignaturasAbierto = false;
  busquedaAsignaturas = '';

  // Estructura visual de componentes, actividades y productos del F_AA_233A.
  modulos: ModuloFormulario[] = [
    {
      nombre: '',
      descripcion: '',
      productos: '',
      estudiante: '',
      actividades: [
        { nombre: '', horas: 0 },
        { nombre: '', horas: 0 }
      ]
    }
  ];

  // Estado de la interfaz
  cargando = false;
  guardando = false;
  enviando = false;
  mensajeError = '';
  mensajeExito = '';
  mostrarModalConfirmacion = false;

  // Propuesta actual (si estamos editando)
  propuestaId: number | null = null;
  estadoPropuesta: string = 'BORRADOR'; // HU03 T09: Para deshabilitar campos si estÃ¡ PENDIENTE
  puedeEditar = true; // HU05 T15: Indica si se puede editar (BORRADOR u OBSERVADA)
  noEditable = false; // HU05 T15: Indica si NO se puede editar

  // HU04 T11-T13: Observaciones CPGIC
  observaciones: any[] = []; // Lista de observaciones
  tieneObservaciones = false; // Indica si hay observaciones
  reEnviando = false; // Estado de reenvÃ­o
  mostrarModalReenvio = false; // Modal de confirmaciÃ³n para reenvÃ­o
  mostrarModalCancelar = false;
  mostrarModalEliminarElemento = false;
  tipoElementoEliminar: 'modulo' | 'actividad' | null = null;
  indiceModuloEliminar: number | null = null;
  indiceActividadEliminar: number | null = null;

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
    console.log(' T02: Iniciando componente FormularioPropuestaComponent');
    
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
        console.log(' T02: Asignaturas cargadas:', asignaturas.length);
      },
      error: (error) => {
        console.error(' T02: Error cargando asignaturas:', error);
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
        if (docentes.length && !this.docenteSeleccionado) {
          this.formulario.patchValue({ profesorId: docentes[0].id });
        }
        console.log(' T02: Docentes cargados:', docentes.length);
      },
      error: (error) => {
        console.error(' T02: Error cargando docentes:', error);
        this.mensajeError = 'Error al cargar docentes';
      }
    });
  }

  /**
   * T02: Cargar propuesta existente para ediciÃ³n
   * HU05 T15: Permite ediciÃ³n en estado BORRADOR u OBSERVADA
   */
  cargarPropuesta(id: number): void {
    this.cargando = true;
    this.propuestaService.obtenerPropuestaById(id).subscribe({
      next: (propuesta) => {
        console.log(' T02: Propuesta cargada:', propuesta);
        
        // Guardar estado
        this.estadoPropuesta = propuesta.estado;
        
        // HU05 T15: Permitir ediciÃ³n solo en BORRADOR u OBSERVADA
        const estadosEditables = ['BORRADOR', 'OBSERVADA'];
        this.puedeEditar = estadosEditables.includes(propuesta.estado);
        this.noEditable = !this.puedeEditar;
        
        // Llenar formulario con datos de la propuesta
        this.formulario.patchValue({
          nombreProyecto: this.texto(propuesta.nombreProyecto),
          numeroParticipantes: propuesta.numeroParticipantes,
          profesorId: propuesta.profesorId,
          descripcion: this.texto(propuesta.descripcion),
          objetivo: this.texto(propuesta.objetivo),
          alcance: this.texto(propuesta.alcance),
          asignaturaIds: propuesta.asignaturas.map(a => a.id)
        });

        this.cargarModulosDesdePropuesta(propuesta);

        // HU05 T15: Deshabilitar campos si NO se puede editar
        if (!this.puedeEditar) {
          this.formulario.disable();
          this.mensajeError = ` No se puede editar una propuesta en estado '${propuesta.estado}'. ` +
                             `Solo se pueden editar propuestas en estado BORRADOR u OBSERVADA.`;
        }

        // HU04 T11: Cargar observaciones si propuesta estÃ¡ OBSERVADA
        if (propuesta.estado === 'OBSERVADA') {
          this.cargarObservaciones(id);
        }
      },
      error: (error) => {
        console.error(' T02: Error cargando propuesta:', error);
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
        console.log(' HU04 T11: Observaciones cargadas:', observaciones.length);
      },
      error: (error) => {
        console.error(' HU04 T11: Error cargando observaciones:', error);
        // No mostrar error si no hay observaciones, es normal
        this.observaciones = [];
        this.tieneObservaciones = false;
      }
    });
  }

  /**
   * HU04 T12: Reenviar propuesta despuÃ©s de correcciones
   * Limpia observaciones y cambia estado OBSERVADA a PENDIENTE
   */
  reenviarPropuesta(): void {
    if (!this.propuestaId) {
      this.mensajeError = 'Error: ID de propuesta no disponible';
      return;
    }

    // Mostrar modal de confirmaciÃ³n
    this.mostrarModalReenvio = true;
  }

  /**
   * HU04 T12: Confirmar reenvÃ­o
   */
  confirmarReenvio(): void {
    this.mostrarModalReenvio = false;

    if (!this.propuestaId) {
      this.mensajeError = 'Error: ID de propuesta no disponible';
      return;
    }

    this.reEnviando = true;
    this.mensajeError = '';

    console.log(' HU04 T12: Reenviando propuesta', this.propuestaId);

    const datos = this.construirPropuesta();

    this.propuestaService.actualizarPropuesta(this.propuestaId, datos).subscribe({
      next: () => {
        this.ejecutarReenvioObservada(this.propuestaId as number);
      },
      error: (error) => {
        console.error(' HU04 T12: Error guardando antes de reenviar:', error);
        this.mensajeError = error.message || 'Error al guardar la propuesta corregida';
        this.reEnviando = false;
      }
    });
  }

  /**
   * HU04 T12: Ejecuta el reenvÃ­o una vez guardadas las correcciones.
   */
  private ejecutarReenvioObservada(propuestaId: number): void {
    this.propuestaService.reenviarDespuesDeObservaciones(propuestaId).subscribe({
      next: (propuesta) => {
        console.log(' HU04 T12: Propuesta reenviada:', propuesta);
        this.mensajeExito = 'Propuesta corregida reenviada a revisiÃ³n exitosamente';
        this.estadoPropuesta = propuesta.estado;
        
        // Limpiar observaciones y deshabilitar formulario
        this.observaciones = [];
        this.tieneObservaciones = false;
        this.formulario.disable();
        
        // Redirigir al dashboard despuÃ©s de 1.5 segundos
        setTimeout(() => {
          this.router.navigate(['/tablero']);
        }, 1500);
      },
      error: (error) => {
        console.error(' HU04 T12: Error reenviando:', error);
        this.mensajeError = error.message || 'Error al reenviar la propuesta';
      },
      complete: () => {
        this.reEnviando = false;
      }
    });
  }

  /**
   * HU04 T12: Cancelar modal de reenvÃ­o
   */
  cancelarReenvio(): void {
    this.mostrarModalReenvio = false;
  }

  /**
   * T01: Guardar como borrador
   * HU05 T16: Guardar cambios de propuesta editada
   * - NO requiere validaciÃ³n - permite guardar incompleto
   * - Estado: BORRADOR
   * - Permite guardar incompleto (puede estar vacÃ­o)
   */
  guardarBorrador(): void {
    // SIN VALIDACIÃ“N - permitir guardar incompleto en estado BORRADOR
    this.guardando = true;
    this.mensajeError = '';

    const datos = this.construirPropuesta();
    
    console.log(' T01/HU05: Guardando como borrador:', datos);

    const operacion = this.propuestaId 
      ? this.propuestaService.actualizarPropuesta(this.propuestaId, datos)
      : this.propuestaService.guardarBorrador(datos as CreatePropuestaRequest);

    operacion.subscribe({
      next: (propuesta) => {
        console.log(' T01/HU05: Borrador/Propuesta guardado exitosamente:', propuesta);
        this.mensajeExito = this.propuestaId 
          ? ' Cambios guardados exitosamente'
          : ' Propuesta guardada como borrador';
        
        // Si fue creaciÃ³n, actualizar el ID
        if (!this.propuestaId) {
          this.propuestaId = propuesta.id;
        }

        // Redirigir al dashboard despuÃ©s de 1.5 segundos
        setTimeout(() => {
          this.router.navigate(['/tablero']);
        }, 1500);
      },
      error: (error) => {
        console.error(' T01/HU05: Error guardando:', error);
        this.mensajeError = error.message || 'Error al guardar la propuesta';
      },
      complete: () => {
        this.guardando = false;
      }
    });
  }

  /**
   * HU03 T08: Enviar propuesta a revisiÃ³n
   * - Valida que el formulario estÃ© completo
   * - Muestra modal de confirmaciÃ³n
   * - Llama al nuevo endpoint POST /api/propuestas/{id}/enviar-revision
   * - El backend valida completamente antes de cambiar estado
   */
  enviarPropuesta(): void {
    // ValidaciÃ³n frontend
    if (this.formulario.invalid) {
      this.mostrarErroresValidacion();
      return;
    }

    if (this.estadoPropuesta === 'OBSERVADA') {
      this.mostrarModalReenvio = true;
      return;
    }

    // Mostrar modal de confirmaciÃ³n
    this.mostrarModalConfirmacion = true;
  }

  /**
   * HU03 T09: Confirmar envÃ­o a revisiÃ³n
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
          console.error(' HU03: Error creando propuesta:', error);
          this.mensajeError = error.message || 'Error al crear la propuesta';
        }
      });
    } else {
      const datos = this.construirPropuesta();
      this.propuestaService.actualizarPropuesta(this.propuestaId, datos).subscribe({
        next: () => this.enviarARevision(this.propuestaId as number),
        error: (error) => {
          console.error(' HU03: Error guardando propuesta antes de enviar:', error);
          this.mensajeError = error.message || 'Error al guardar la propuesta antes de enviar';
        }
      });
    }
  }

  /**
   * HU03 T07: Llamar al nuevo endpoint de envÃ­o a revisiÃ³n
   */
  private enviarARevision(propuestaId: number): void {
    this.enviando = true;
    this.mensajeError = '';

    console.log(' HU03: Enviando propuesta', propuestaId, 'a revisiÃ³n');

    this.propuestaService.enviarARevision(propuestaId).subscribe({
      next: (propuesta) => {
        console.log(' HU03: Propuesta enviada a revisiÃ³n:', propuesta);
        this.mensajeExito = ' Propuesta enviada a revisiÃ³n exitosamente';
        this.estadoPropuesta = propuesta.estado;
        
        // Deshabilitar campos despuÃ©s del envÃ­o
        this.formulario.disable();
        
        // Redirigir al dashboard despuÃ©s de 1.5 segundos
        setTimeout(() => {
          this.router.navigate(['/tablero']);
        }, 1500);
      },
      error: (error) => {
        console.error(' HU03: Error enviando a revisiÃ³n:', error);
        this.mensajeError = error.message || 'Error al enviar la propuesta a revisiÃ³n';
        
        // Mostrar errores especÃ­ficos del backend
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
   * HU03 T09: Mostrar errores de validaciÃ³n especÃ­ficos
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
   * Cancelar modal de confirmaciÃ³n
   */
  cancelarEnvio(): void {
    this.mostrarModalConfirmacion = false;
  }

  /**
   * Cancelar ediciÃ³n y volver atrÃ¡s
   */
  cancelar(): void {
    this.mostrarModalCancelar = true;
  }

  /**
   * Cierra el modal de cancelacion y permite continuar editando.
   */
  continuarEditando(): void {
    this.mostrarModalCancelar = false;
  }

  /**
   * Confirma la salida sin guardar cambios.
   */
  confirmarCancelar(): void {
    this.mostrarModalCancelar = false;
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
      asignaturaIds: valores.asignaturaIds || [],
      componentes: this.construirComponentes()
    };
  }

  /**
   * Construye los componentes, actividades y productos esperados para enviarlos al backend.
   */
  private construirComponentes(): any[] {
    return this.modulos
      .map((modulo, index) => ({
        nombre: modulo.nombre.trim(),
        descripcion: modulo.descripcion.trim(),
        orden: index + 1,
        nombresEstudiante: modulo.estudiante.trim(),
        actividades: modulo.actividades
          .map((actividad, actividadIndex) => ({
            numero: actividadIndex + 1,
            descripcion: actividad.nombre.trim(),
            horas: Number(actividad.horas || 0)
          }))
          .filter(actividad => actividad.descripcion || actividad.horas > 0),
        productosEsperados: modulo.productos
          .split(/\r?\n/)
          .map(producto => ({ descripcion: producto.replace(/^[-\d.\s]+/, '').trim() }))
          .filter(producto => producto.descripcion)
      }))
      .filter(modulo =>
        modulo.nombre ||
        modulo.descripcion ||
        modulo.actividades.length ||
        modulo.productosEsperados.length
      );
  }

  /**
   * Carga componentes guardados desde una propuesta existente.
   */
  private cargarModulosDesdePropuesta(propuesta: any): void {
    const componentes = propuesta.componentes || propuesta.componentesDto || [];

    if (!componentes.length) {
      this.modulos = [
        {
          nombre: '',
          descripcion: '',
          productos: '',
          estudiante: '',
          actividades: [
            { nombre: '', horas: 0 },
            { nombre: '', horas: 0 }
          ]
        }
      ];
      return;
    }

    this.modulos = componentes
      .sort((a: any, b: any) => (a.orden || 0) - (b.orden || 0))
      .map((componente: any) => ({
        nombre: this.texto(componente.nombre || ''),
        descripcion: this.texto(componente.descripcion || ''),
        productos: (componente.productosEsperados || [])
          .map((producto: any) => this.texto(producto.descripcion))
          .filter(Boolean)
          .join('\n'),
        estudiante: this.texto(componente.estudiante?.nombresEstudiante || componente.estudiante?.nombres || ''),
        actividades: (componente.actividades?.length ? componente.actividades : [{ descripcion: '', horas: 0 }])
          .sort((a: any, b: any) => (a.numero || 0) - (b.numero || 0))
          .map((actividad: any) => ({
            nombre: this.texto(actividad.descripcion || ''),
            horas: Number(actividad.horas || 0)
          }))
      }));
  }

  /**
   * Obtener mensaje de error para campo especÃ­fico
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
      return `${this.obtenerLabelCampo(nombreCampo)} debe tener mÃ­nimo ${campo.errors['minlength'].requiredLength} caracteres`;
    }
    if (campo.errors['maxlength']) {
      return `${this.obtenerLabelCampo(nombreCampo)} debe tener mÃ¡ximo ${campo.errors['maxlength'].requiredLength} caracteres`;
    }
    if (campo.errors['min']) {
      return `${this.obtenerLabelCampo(nombreCampo)} debe ser mÃ­nimo ${campo.errors['min'].min}`;
    }
    if (campo.errors['max']) {
      return `${this.obtenerLabelCampo(nombreCampo)} debe ser mÃ¡ximo ${campo.errors['max'].max}`;
    }

    return 'Error en este campo';
  }

  /**
   * Obtener etiqueta legible del campo
   */
  private obtenerLabelCampo(nombreCampo: string): string {
    const labels: { [key: string]: string } = {
      nombreProyecto: 'Nombre del proyecto',
      numeroParticipantes: 'NÃºmero de participantes',
      profesorId: 'Profesor',
      descripcion: 'DescripciÃ³n',
      objetivo: 'Objetivo',
      alcance: 'Alcance',
      asignaturaIds: 'Asignaturas'
    };

    return labels[nombreCampo] || nombreCampo;
  }

  /**
   * Verificar si campo es invÃ¡lido y ha sido tocado
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
      // Agregar si no estÃ¡
      newValue = currentValue.includes(asignaturaId) 
        ? currentValue 
        : [...currentValue, asignaturaId];
    } else {
      // Remover si estÃ¡
      newValue = currentValue.filter((id: number) => id !== asignaturaId);
    }

    control.setValue(newValue);
    control.markAsTouched();
  }

  /**
   * Verifica si una asignatura estÃ¡ seleccionada.
   */
  asignaturaSeleccionada(asignaturaId: number): boolean {
    const seleccionadas = this.formulario.get('asignaturaIds')?.value || [];
    return seleccionadas.includes(asignaturaId);
  }

  /**
   * Mantiene estable el DOM de listas mientras se escribe en mÃ³dulos y actividades.
   */
  trackByIndex(index: number): number {
    return index;
  }

  /**
   * Abre o cierra la lista desplegable de asignaturas.
   */
  toggleMenuAsignaturas(): void {
    this.menuAsignaturasAbierto = !this.menuAsignaturasAbierto;
  }

  /**
   * Actualiza el filtro visible de asignaturas.
   */
  buscarAsignaturas(valor: string): void {
    this.busquedaAsignaturas = valor;
  }

  /**
   * Limpia el buscador de asignaturas.
   */
  limpiarBusquedaAsignaturas(): void {
    this.busquedaAsignaturas = '';
  }

  /**
   * Asignaturas visibles segun el texto ingresado en el buscador.
   */
  get asignaturasFiltradas(): Asignatura[] {
    const busqueda = this.normalizarTexto(this.busquedaAsignaturas);

    if (!busqueda) {
      return this.asignaturas;
    }

    return this.asignaturas.filter(asignatura => {
      const codigo = this.normalizarTexto(asignatura.codigo);
      const nombre = this.normalizarTexto(asignatura.nombre);
      return codigo.includes(busqueda) || nombre.includes(busqueda);
    });
  }

  /**
   * Obtiene el texto visible del selector de asignaturas.
   */
  get resumenAsignaturas(): string {
    const seleccionadas = this.formulario.get('asignaturaIds')?.value || [];

    if (!seleccionadas.length) {
      return 'Seleccione asignaturas del menú...';
    }

    if (seleccionadas.length === 1) {
      const asignatura = this.asignaturas.find(item => item.id === seleccionadas[0]);
      return asignatura ? `${this.texto(asignatura.codigo)} - ${this.texto(asignatura.nombre)}` : '1 asignatura seleccionada';
    }

    return `${seleccionadas.length} asignaturas seleccionadas`;
  }

  /**
   * Obtiene las asignaturas seleccionadas para mostrarlas como etiquetas.
   */
  get asignaturasSeleccionadas(): Asignatura[] {
    const seleccionadas = this.formulario.get('asignaturaIds')?.value || [];
    return this.asignaturas.filter(asignatura => seleccionadas.includes(asignatura.id));
  }

  private normalizarTexto(valor: unknown): string {
    return this.texto(valor)
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .toLowerCase()
      .trim();
  }

  /**
   * Construye el nombre completo del docente para mostrarlo en select y tablas.
   */
  nombreDocente(docente: Docente): string {
    const titulo = docente.tituloAcademico ? `${docente.tituloAcademico} ` : '';
    return `${titulo}${docente.nombres} ${docente.apellidos}`.trim();
  }

  /**
   * Obtiene el docente seleccionado en el formulario.
   */
  private get docenteSeleccionado(): Docente | undefined {
    const profesorId = Number(this.formulario.get('profesorId')?.value);
    return this.docentes.find(docente => docente.id === profesorId);
  }

  /**
   * Obtiene el nombre del profesor para la secciÃ³n Presentado por.
   */
  get nombreProfesorSeleccionado(): string {
    const docente = this.docenteSeleccionado;
    return docente ? `${docente.nombres} ${docente.apellidos}` : 'Profesor proponente';
  }

  /**
   * Obtiene el nombre completo del profesor con tÃ­tulo acadÃ©mico para el campo de solo lectura.
   */
  get nombreDocenteCompletoSeleccionado(): string {
    const docente = this.docenteSeleccionado;
    return docente ? this.nombreDocente(docente) : 'Cargando profesor...';
  }

  /**
   * Obtiene el tÃ­tulo acadÃ©mico del profesor seleccionado.
   */
  get tituloProfesorSeleccionado(): string {
    return this.docenteSeleccionado?.tituloAcademico || 'Docente, PhD.';
  }

  /**
   * Resume estudiantes ingresados en los mÃ³dulos visuales.
   */
  get resumenEstudiantes(): string {
    return this.estudiantesPropuestos.length ? this.estudiantesPropuestos.join(', ') : '(sin estudiantes asignados)';
  }

  /**
   * Obtiene estudiantes propuestos desde los mÃ³dulos visuales para listarlos en aprobaciones.
   */
  get estudiantesPropuestos(): string[] {
    return this.modulos
      .map(modulo => modulo.estudiante.trim())
      .filter(Boolean);
  }

  /**
   * Obtiene observaciones asociadas visualmente a una seccion del formulario.
   */
  observacionesPorSeccion(seccion: 'datos' | 'descripcion' | 'objetivo' | 'alcance' | 'componentes' | 'general'): any[] {
    return this.deduplicarObservaciones(this.observaciones)
      .filter(obs => this.obtenerSeccionObservacion(obs) === seccion);
  }

  /**
   * Devuelve el titulo visual segun el tipo de observacion.
   */
  tituloObservacion(obs: any): string {
    return this.esObservacionNuevaAprobacion(obs)
      ? 'Observación para el miembro de la CPGIC'
      : 'Observación del director';
  }

  /**
   * Devuelve el texto visible de la observacion sin prefijos internos.
   */
  textoObservacion(obs: any): string {
    return this.texto(obs?.observacion || '')
      .replace(/^Observacion para el miembro de la CPGIC:\s*/i, '')
      .replace(/^Solicitud de nueva aprobacion:\s*/i, '');
  }

  /**
   * Resume quien realizo una observacion y cuando.
   */
  resumenObservacion(obs: any): string {
    const quien = this.texto(obs?.realizadoPor || 'Director');
    const fecha = obs?.fechaObservacion ? new Date(obs.fechaObservacion).toLocaleDateString('es-EC') : '';

    if (quien && fecha) {
      return `${quien} - ${fecha}`;
    }

    return fecha || quien;
  }

  /**
   * Clasifica observaciones usando el texto disponible en la base actual.
   */
  private obtenerSeccionObservacion(obs: any): 'datos' | 'descripcion' | 'objetivo' | 'alcance' | 'componentes' | 'general' {
    const texto = (obs?.seccion || obs?.campo || obs?.observacion || '').toLowerCase();

    if (/(descripci[oó]n|problem[aá]tica|problematica)/.test(texto)) {
      return 'descripcion';
    }

    if (/(objetivo)/.test(texto)) {
      return 'objetivo';
    }

    if (/(alcance)/.test(texto)) {
      return 'alcance';
    }

    if (/(actividad|actividades|m[oó]dulo|modulo|producto|productos|hora|horas|componente)/.test(texto)) {
      return 'componentes';
    }

    if (/(asignatura|participante|participantes|proyecto|t[ií]tulo|titulo|datos generales|departamento|carrera)/.test(texto)) {
      return 'datos';
    }

    return 'general';
  }

  /**
   * Evita mostrar varias veces el aviso automatico de nueva aprobacion.
   */
  private deduplicarObservaciones(observaciones: any[]): any[] {
    let yaIncluyoNuevaAprobacion = false;

    return observaciones.filter(obs => {
      if (!this.esObservacionNuevaAprobacion(obs)) {
        return true;
      }

      if (yaIncluyoNuevaAprobacion) {
        return false;
      }

      yaIncluyoNuevaAprobacion = true;
      return true;
    });
  }

  /**
   * Identifica el aviso automatico generado al reasignar estudiantes.
   */
  private esObservacionNuevaAprobacion(obs: any): boolean {
    const texto = (obs?.observacion || '').toLowerCase();
    return texto.includes('solicitud de nueva aprobacion') ||
      texto.includes('observacion para el miembro de la cpgic');
  }

  /**
   * Agrega un mÃ³dulo visual al formulario.
   */
  agregarModulo(): void {
    this.modulos = [
      ...this.modulos,
      {
        nombre: '',
        descripcion: '',
        productos: '',
        estudiante: '',
        actividades: [
          { nombre: '', horas: 0 },
          { nombre: '', horas: 0 }
        ]
      }
    ];
  }

  /**
   * Elimina un mÃ³dulo visual del formulario.
   */
  eliminarModulo(index: number): void {
    if (this.modulos.length === 1) {
      return;
    }

    this.tipoElementoEliminar = 'modulo';
    this.indiceModuloEliminar = index;
    this.indiceActividadEliminar = null;
    this.mostrarModalEliminarElemento = true;
  }

  /**
   * Actualiza una propiedad simple de un mÃ³dulo visual.
   */
  actualizarModulo(index: number, campo: 'nombre' | 'descripcion' | 'productos' | 'estudiante', valor: string): void {
    this.modulos[index][campo] = valor;
  }

  /**
   * Agrega una actividad visual dentro de un mÃ³dulo.
   */
  agregarActividad(indexModulo: number): void {
    this.modulos[indexModulo].actividades.push({ nombre: '', horas: 0 });
  }

  /**
   * Elimina una actividad visual dentro de un mÃ³dulo.
   */
  eliminarActividad(indexModulo: number, indexActividad: number): void {
    const actividades = this.modulos[indexModulo].actividades;

    if (actividades.length === 1) {
      return;
    }

    this.tipoElementoEliminar = 'actividad';
    this.indiceModuloEliminar = indexModulo;
    this.indiceActividadEliminar = indexActividad;
    this.mostrarModalEliminarElemento = true;
  }

  /**
   * Cancela la eliminacion de un modulo o actividad.
   */
  cancelarEliminacionElemento(): void {
    this.mostrarModalEliminarElemento = false;
    this.tipoElementoEliminar = null;
    this.indiceModuloEliminar = null;
    this.indiceActividadEliminar = null;
  }

  /**
   * Confirma la eliminacion de un modulo o actividad del formulario.
   */
  confirmarEliminacionElemento(): void {
    if (this.tipoElementoEliminar === 'modulo' && this.indiceModuloEliminar !== null) {
      this.modulos = this.modulos.filter((_, posicion) => posicion !== this.indiceModuloEliminar);
    }

    if (
      this.tipoElementoEliminar === 'actividad' &&
      this.indiceModuloEliminar !== null &&
      this.indiceActividadEliminar !== null
    ) {
      this.modulos[this.indiceModuloEliminar].actividades.splice(this.indiceActividadEliminar, 1);
    }

    this.cancelarEliminacionElemento();
  }

  /**
   * Actualiza una actividad visual dentro de un mÃ³dulo.
   */
  actualizarActividad(indexModulo: number, indexActividad: number, campo: 'nombre' | 'horas', valor: string): void {
    const actividad = this.modulos[indexModulo].actividades[indexActividad];

    if (campo === 'horas') {
      actividad.horas = Number(valor) || 0;
      return;
    }

    actividad.nombre = valor;
  }

  /**
   * Calcula el total de horas de un mÃ³dulo visual.
   */
  totalHorasModulo(modulo: ModuloFormulario): number {
    return modulo.actividades.reduce((total, actividad) => total + Number(actividad.horas || 0), 0);
  }

  /**
   * Aplica formato textual a campos principales del formulario.
   */
  aplicarFormatoCampo(nombreCampo: string, formato: CampoFormato, textarea: HTMLTextAreaElement): void {
    const control = this.formulario.get(nombreCampo);

    if (!control) {
      return;
    }

    const resultado = this.formatearTexto(textarea, formato);
    control.setValue(resultado.valor);
    control.markAsDirty();
    this.restaurarSeleccion(textarea, resultado.inicio, resultado.fin);
  }

  /**
   * Aplica formato textual a campos visuales de mÃ³dulos.
   */
  aplicarFormatoModulo(index: number, campo: 'descripcion' | 'productos', formato: CampoFormato, textarea: HTMLTextAreaElement): void {
    const resultado = this.formatearTexto(textarea, formato);
    this.modulos[index][campo] = resultado.valor;
    this.restaurarSeleccion(textarea, resultado.inicio, resultado.fin);
  }

  /**
   * ContinÃºa listas numeradas o viÃ±etas al presionar Enter en campos principales.
   */
  manejarEnterCampo(evento: KeyboardEvent, nombreCampo: string, textarea: HTMLTextAreaElement): void {
    const control = this.formulario.get(nombreCampo);
    const resultado = this.procesarEnterLista(evento, textarea);

    if (!control || !resultado) {
      return;
    }

    control.setValue(resultado.valor);
    control.markAsDirty();
    this.restaurarSeleccion(textarea, resultado.inicio, resultado.fin);
  }

  /**
   * ContinÃºa listas numeradas o viÃ±etas al presionar Enter en campos de mÃ³dulos.
   */
  manejarEnterModulo(evento: KeyboardEvent, index: number, campo: 'descripcion' | 'productos', textarea: HTMLTextAreaElement): void {
    const resultado = this.procesarEnterLista(evento, textarea);

    if (!resultado) {
      return;
    }

    this.modulos[index][campo] = resultado.valor;
    this.restaurarSeleccion(textarea, resultado.inicio, resultado.fin);
  }

  /**
   * Inserta marcas simples de formato en el texto seleccionado.
   */
  private formatearTexto(textarea: HTMLTextAreaElement, formato: CampoFormato): { valor: string; inicio: number; fin: number } {
    const valor = textarea.value;
    const inicio = textarea.selectionStart;
    const fin = textarea.selectionEnd;
    const seleccionado = valor.slice(inicio, fin);

    if (formato === 'numerada' || formato === 'vineta') {
      const numeroInicial = this.obtenerSiguienteNumero(valor, inicio);
      const prefijo = formato === 'numerada' ? `${numeroInicial}. ` : '- ';
      const textoBase = seleccionado || 'Texto';
      const lineas = textoBase.split('\n').map((linea, indice) =>
        formato === 'numerada' ? `${numeroInicial + indice}. ${linea}` : `${prefijo}${linea}`
      );
      const reemplazo = lineas.join('\n');
      return {
        valor: `${valor.slice(0, inicio)}${reemplazo}${valor.slice(fin)}`,
        inicio,
        fin: inicio + reemplazo.length
      };
    }

    const marca = formato === 'negrita' ? '**' : '*';
    const reemplazo = `${marca}${seleccionado || 'Texto'}${marca}`;

    return {
      valor: `${valor.slice(0, inicio)}${reemplazo}${valor.slice(fin)}`,
      inicio: inicio + marca.length,
      fin: inicio + reemplazo.length - marca.length
    };
  }

  /**
   * Devuelve el foco al textarea despuÃ©s de aplicar formato.
   */
  private restaurarSeleccion(textarea: HTMLTextAreaElement, inicio: number, fin: number): void {
    setTimeout(() => {
      textarea.focus();
      textarea.setSelectionRange(inicio, fin);
    });
  }

  /**
   * Procesa Enter dentro de listas y devuelve el texto actualizado.
   */
  private procesarEnterLista(evento: KeyboardEvent, textarea: HTMLTextAreaElement): { valor: string; inicio: number; fin: number } | null {
    if (evento.key !== 'Enter' || evento.shiftKey) {
      return null;
    }

    const valor = textarea.value;
    const inicio = textarea.selectionStart;
    const fin = textarea.selectionEnd;
    const inicioLinea = valor.lastIndexOf('\n', inicio - 1) + 1;
    const lineaActual = valor.slice(inicioLinea, inicio);
    const lineaCompleta = valor.slice(inicioLinea, valor.indexOf('\n', inicio) === -1 ? valor.length : valor.indexOf('\n', inicio));
    const numerada = lineaActual.match(/^(\s*)(\d+)\.\s(.*)$/);
    const vineta = lineaActual.match(/^(\s*)-\s(.*)$/);

    if (!numerada && !vineta) {
      return null;
    }

    evento.preventDefault();

    if (numerada) {
      const indentacion = numerada[1];
      const numero = Number(numerada[2]);
      const contenido = lineaCompleta.replace(/^(\s*)\d+\.\s/, '').trim();

      if (!contenido) {
        const valorSinMarcador = `${valor.slice(0, inicioLinea)}${valor.slice(fin)}`;
        return { valor: valorSinMarcador, inicio: inicioLinea, fin: inicioLinea };
      }

      const insercion = `\n${indentacion}${numero + 1}. `;
      return {
        valor: `${valor.slice(0, inicio)}${insercion}${valor.slice(fin)}`,
        inicio: inicio + insercion.length,
        fin: inicio + insercion.length
      };
    }

    const indentacion = vineta?.[1] || '';
    const contenido = lineaCompleta.replace(/^(\s*)-\s/, '').trim();

    if (!contenido) {
      const valorSinMarcador = `${valor.slice(0, inicioLinea)}${valor.slice(fin)}`;
      return { valor: valorSinMarcador, inicio: inicioLinea, fin: inicioLinea };
    }

    const insercion = `\n${indentacion}- `;
    return {
      valor: `${valor.slice(0, inicio)}${insercion}${valor.slice(fin)}`,
      inicio: inicio + insercion.length,
      fin: inicio + insercion.length
    };
  }

  /**
   * Calcula el siguiente nÃºmero de lista antes del cursor.
   */
  private obtenerSiguienteNumero(valor: string, posicion: number): number {
    const lineasPrevias = valor.slice(0, posicion).split(/\r?\n/).reverse();

    for (const linea of lineasPrevias) {
      const coincidencia = linea.match(/^\s*(\d+)\.\s/);
      if (coincidencia) {
        return Number(coincidencia[1]) + 1;
      }
    }

    return 1;
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
      .replace(/Ã/g, 'Á')
      .replace(/Ã‰/g, 'É')
      .replace(/Ã/g, 'Í')
      .replace(/Ã“/g, 'Ó')
      .replace(/Ãš/g, 'Ú')
      .replace(/Ã‘/g, 'Ñ')
      .replace(/Â¿/g, '¿')
      .replace(/Â¡/g, '¡')
      .replace(/Â°/g, '°')
      .replace(/Âº/g, 'º')
      .replace(/Âª/g, 'ª')
      .replace(/Â·/g, '·')
      .replace(/Â/g, '')
      .replace(/â€”/g, '—')
      .replace(/â€“/g, '–')
      .replace(/â€˜/g, '‘')
      .replace(/â€™/g, '’')
      .replace(/â€œ/g, '“')
      .replace(/â€/g, '”');
  }
}

