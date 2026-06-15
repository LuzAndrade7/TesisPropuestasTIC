import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PropuestaService } from '../../services/propuesta.service';
import { AsignarEstudiantesComponent } from '../asignar-estudiantes/asignar-estudiantes.component';

/**
 * HU06 T18 + HU07 T24-T25: Componente para visualizar detalle completo de propuesta
 * Muestra: datos bÃ¡sicos, profesor, asignaturas, observaciones, histÃ³rico de estados
 * Botones contextuales segÃºn estado
 */
@Component({
  selector: 'app-detalle-propuesta',
  standalone: true,
  imports: [CommonModule, AsignarEstudiantesComponent],
  templateUrl: './detalle-propuesta.component.html',
  styleUrls: ['./detalle-propuesta.component.scss']
})
export class DetallePropuestaComponent implements OnInit, OnDestroy {
  propuestaId: number | null = null;
  detalle: any = null;
  
  // Estados de carga
  cargando = true;
  error: string | null = null;
  
  // HU07 T24-T25: Control del modal de asignaciÃ³n
  mostrarModalAsignarEstudiantes = false;
  estudiantesAsignados: any[] = [];
  
  // Colores para estados
  estadoColores: { [key: string]: string } = {
    'BORRADOR': '#0E2240',      // Navy
    'PENDIENTE': '#77A4DC',     // Blue
    'OBSERVADA': '#F3BD46',     // Gold
    'APROBADA': '#28a745',      // Green
    'RECHAZADA': '#E31D1A'      // Red
  };

  private destroy$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private propuestaService: PropuestaService,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit(): void {
    this.route.params.pipe(takeUntil(this.destroy$)).subscribe(params => {
      this.propuestaId = params['id'];
      if (this.propuestaId) {
        this.cargarDetalleCompleto(this.propuestaId);
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Carga el detalle completo de la propuesta desde el API
   */
  private cargarDetalleCompleto(id: number): void {
    this.cargando = true;
    this.error = null;
    
    this.propuestaService.obtenerDetalleCompleto(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe(
        (data) => {
          this.detalle = data;
          this.cargando = false;
          this.cargarEstudiantesAsignados(id);
        },
        (error) => {
          this.error = error?.message || 'Error al cargar detalle de la propuesta';
          this.cargando = false;
          console.error('Error loading detalle:', error);
        }
      );
  }

  /**
   * HU07 T24: Carga estudiantes ya asignados a la propuesta
   */
  private cargarEstudiantesAsignados(propuestaId: number): void {
    this.propuestaService.obtenerEstudiantesAsignados(propuestaId)
      .pipe(takeUntil(this.destroy$))
      .subscribe(
        (estudiantes: any[]) => {
          this.estudiantesAsignados = estudiantes;
        },
        (error) => {
          console.error('Error cargando estudiantes asignados:', error);
        }
      );
  }

  /**
   * Obtiene el color para un estado especÃ­fico
   */
  getColorEstado(estado: string): string {
    return this.estadoColores[estado] || '#0E2240';
  }

  /**
   * Verifica si se puede editar la propuesta
   */
  get puedeEditar(): boolean {
    return this.detalle?.puedeEditar === true;
  }

  /**
   * Verifica si se puede reenviar la propuesta
   */
  get puedeReenviar(): boolean {
    return this.detalle?.puedeReenviar === true;
  }

  /**
   * Verifica si se puede eliminar la propuesta
   */
  get puedeEliminar(): boolean {
    return this.detalle?.puedeEliminar === true;
  }

  /**
   * HU07 T24: Verifica si se puede asignar estudiantes
   * Solo APROBADA o PENDIENTE
   */
  get puedeAsignarEstudiantes(): boolean {
    return this.detalle?.estado === 'APROBADA';
  }

  /**
   * HU07 T25: Verifica si se puede solicitar nueva aprobaciÃ³n
   * Solo APROBADA
   */
  get puedeSolicitarNuevaAprobacion(): boolean {
    return this.detalle?.estado === 'APROBADA';
  }

  /**
   * Obtiene el nombre del profesor con formato de detalle.
   */
  get nombreProfesorDetalle(): string {
    const profesor = this.detalle?.profesor;

    if (!profesor) {
      return 'Profesor proponente';
    }

    return this.texto(profesor.nombre || `${profesor.nombres || ''} ${profesor.apellidos || ''}`.trim());
  }

  /**
   * Obtiene el tÃ­tulo acadÃ©mico del profesor.
   */
  get tituloProfesorDetalle(): string {
    return this.texto(this.detalle?.profesor?.tituloAcademico || 'Docente, PhD.');
  }

  /**
   * Obtiene tÃ­tulo y nombre del profesor para datos generales.
   */
  get nombreProfesorConTituloDetalle(): string {
    const titulo = this.tituloProfesorDetalle;
    const nombre = this.nombreProfesorDetalle;
    return titulo ? `${titulo} ${nombre}` : nombre;
  }

  /**
   * Navega a la pÃ¡gina de ediciÃ³n
   */
  irAEditar(): void {
    if (this.propuestaId) {
      this.router.navigate(['/propuestas', this.propuestaId]);
    }
  }

  /**
   * Navega a la pantalla formal de asignaciÃ³n de estudiantes.
   */
  irAAsignarEstudiantes(): void {
    if (this.propuestaId) {
      this.router.navigate(['/propuestas', this.propuestaId, 'asignar-estudiantes']);
    }
  }

  /**
   * Abre modal de confirmaciÃ³n para envÃ­o a revisiÃ³n
   */
  enviarARevision(): void {
    if (confirm('Â¿Deseas enviar esta propuesta a revisiÃ³n?\n\nUna vez enviada, no podrÃ¡s editarla hasta que recibas observaciones.')) {
      // TODO: Implementar envÃ­o a revisiÃ³n en el servicio
      console.log('Enviar a revisiÃ³n:', this.propuestaId);
    }
  }

  /**
   * Abre modal de confirmaciÃ³n para reenvÃ­o
   */
  reenviarPropuesta(): void {
    if (confirm('Â¿Deseas reenviar esta propuesta corregida?\n\nSe limpiarÃ¡n todas las observaciones y se marcarÃ¡ como PENDIENTE.')) {
      // TODO: Implementar reenvÃ­o en el servicio
      console.log('Reenviar propuesta:', this.propuestaId);
    }
  }

  /**
   * HU08 T26-T28: Elimina propuesta en estado BORRADOR
   * Solicita confirmaciÃ³n antes de ejecutar
   * Navega a tablero despuÃ©s de exitoso
   */
  eliminarPropuesta(): void {
    if (!this.propuestaId) return;

    // Solo permitir eliminaciÃ³n de propuestas BORRADOR
    if (this.detalle?.estado !== 'BORRADOR') {
      alert(' Error: Solo se pueden eliminar propuestas en estado BORRADOR.\n\nEstado actual: ' + this.detalle?.estado);
      return;
    }

    const nombrePropuesta = this.detalle?.titulo || 'Sin tÃ­tulo';
    const confirmacion = confirm(
      ` ELIMINAR PROPUESTA\n\n` +
      `Propuesta: ${nombrePropuesta}\n` +
      `Estado: BORRADOR\n\n` +
      ` ADVERTENCIA: Esta acciÃ³n NO se puede deshacer.\n\n` +
      `Â¿Deseas continuar?`
    );

    if (!confirmacion) {
      return; // Usuario cancelÃ³
    }

    // Llamar al servicio para eliminar
    this.propuestaService.eliminarPropuesta(this.propuestaId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (respuesta: any) => {
          console.log(' HU08 T28: Propuesta eliminada correctamente', respuesta);
          alert(' Propuesta eliminada exitosamente.\n\nSerÃ¡s redirigido al tablero.');
          
          // Navegar al tablero
          setTimeout(() => {
            this.router.navigate(['/tablero']);
          }, 500);
        },
        error: (error: any) => {
          console.error(' HU08 T28: Error al eliminar propuesta', error);
          
          // Manejo de errores especÃ­ficos
          if (error?.status === 403) {
            alert(' Error: Solo se pueden eliminar propuestas en estado BORRADOR.');
          } else if (error?.status === 404) {
            alert(' Error: Propuesta no encontrada.');
          } else if (error?.status === 400) {
            alert(' Error: Datos invÃ¡lidos.');
          } else {
            const mensaje = error?.error?.message || 'Error al eliminar la propuesta';
            alert(' Error: ' + mensaje);
          }
        }
      });
  }

  /**
   * HU07 T24: Abre modal para asignar estudiantes
   */
  abrirModalAsignarEstudiantes(): void {
    this.mostrarModalAsignarEstudiantes = true;
  }

  /**
   * HU07 T24: Cierra modal de asignaciÃ³n
   */
  cerrarModalAsignarEstudiantes(): void {
    this.mostrarModalAsignarEstudiantes = false;
  }

  /**
   * HU07 T24-T25: Manejador cuando se completa la asignaciÃ³n
   */
  onAsignacionCompleta(evento: any): void {
    console.log(' HU07: Asignacion completada', evento);
    this.cerrarModalAsignarEstudiantes();

    if (this.propuestaId) {
      this.cargarDetalleCompleto(this.propuestaId);
    }
  }

  /**
   * HU07 T25: Solicita nueva aprobaciÃ³n para propuesta APROBADA
   * Cambia estado APROBADA a PENDIENTE
   */
  solicitarNuevaAprobacion(): void {
    if (!this.propuestaId) return;

    const motivo = prompt(
      ' Cambios Detectados\n\n' +
      'Esta propuesta estÃ¡ en estado APROBADA. Al solicitar nueva aprobaciÃ³n, pasarÃ¡ a PENDIENTE.\n\n' +
      'Por favor, indica el motivo de la solicitud:'
    );

    if (!motivo || !motivo.trim()) {
      return; // Usuario cancelÃ³
    }

    this.propuestaService.solicitarNuevaAprobacion(this.propuestaId, motivo.trim())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (propuestaActualizada: any) => {
          console.log(' HU07 T25: Nueva aprobaciÃ³n solicitada');
          // Actualizar el detalle local
          this.detalle = propuestaActualizada;
          alert(' Nueva aprobaciÃ³n solicitada correctamente.\n\nLa propuesta cambiÃ³ a estado PENDIENTE.');
          
          // Recargar despuÃ©s de 1 segundo
          setTimeout(() => {
            if (this.propuestaId) {
              this.cargarDetalleCompleto(this.propuestaId);
            }
          }, 1000);
        },
        error: (error: any) => {
          console.error('Error solicitando nueva aprobaciÃ³n:', error);
          alert(' Error: ' + (error?.message || 'No se pudo solicitar nueva aprobaciÃ³n'));
        }
      });
  }

  /**
   * Vuelve al tablero
   */
  volverAlTablero(): void {
    this.router.navigate(['/tablero']);
  }

  /**
   * Formatea una fecha para mostrar
   */
  formatearFecha(fecha: string | Date): string {
    if (!fecha) return '-';
    const date = new Date(fecha);
    return date.toLocaleDateString('es-CO', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  /**
   * Formatea una fecha corta para el encabezado.
   */
  formatearFechaCorta(fecha: string | Date): string {
    if (!fecha) return '-';
    const date = new Date(fecha);
    return date.toLocaleDateString('es-EC', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  /**
   * Devuelve la clase visual del estado.
   */
  claseEstado(estado: string): string {
    return `detalle-propuesta__badge--${(estado || 'BORRADOR').toLowerCase()}`;
  }

  /**
   * Obtiene el nombre legible de un estudiante asignado.
   */
  nombreEstudiante(estudiante: any): string {
    const dato = estudiante?.estudiante || estudiante;
    return estudiante?.nombreCompleto ||
      dato?.nombreCompleto ||
      dato?.nombresEstudiante ||
      `${dato?.nombres || ''} ${dato?.apellidos || ''}`.trim() ||
      dato?.nombre ||
      'Estudiante';
  }

  /**
   * Obtiene la resoluciÃ³n de aprobaciÃ³n para propuestas aprobadas.
   */
  get resolucionCpgicDetalle(): string {
    return this.detalle?.resolucionCpgic || 'Res. 0042-CPGIC-2026';
  }

  /**
   * Obtiene la fecha principal de aprobaciÃ³n.
   */
  get fechaAprobacionDetalle(): string | Date {
    return this.detalle?.fechaAprobacion || this.detalle?.fechaActualizacion || this.detalle?.fechaCreacion;
  }

  /**
   * Devuelve mÃ³dulos reales si existen; si no, genera mÃ³dulos visuales para la asignaciÃ³n.
   */
  get modulosDetalle(): any[] {
    if (this.detalle?.componentes?.length) {
      return this.detalle.componentes;
    }

    return [];
  }

  /**
   * Cuenta mÃ³dulos que todavÃ­a no muestran estudiante asignado.
   */
  get modulosSinEstudiante(): number {
    return this.modulosDetalle.filter(modulo => !this.obtenerEstudianteModulo(modulo)).length;
  }

  /**
   * Obtiene el estudiante asociado visualmente a un mÃ³dulo.
   */
  obtenerEstudianteModulo(modulo: any): any | null {
    if (modulo?.estudiante) {
      return modulo.estudiante;
    }

    if (modulo?.estudianteId) {
      return this.estudiantesAsignados
        .map(asignacion => asignacion?.estudiante || asignacion)
        .find(estudiante => estudiante?.id === modulo.estudianteId) || null;
    }

    return null;
  }

  /**
   * Calcula el total de horas registradas en las actividades de un modulo.
   */
  totalHorasModulo(modulo: any): number {
    return (modulo?.actividades || [])
      .reduce((total: number, actividad: any) => total + Number(actividad?.horas || 0), 0);
  }

  /**
   * Obtiene observaciones asociadas visualmente a una secciÃ³n del formulario.
   */
  observacionesPorSeccion(seccion: 'datos' | 'descripcion' | 'objetivo' | 'alcance' | 'componentes' | 'general'): any[] {
    const observaciones = this.detalle?.observaciones || [];
    return this.deduplicarObservaciones(observaciones)
      .filter((obs: any) => this.obtenerSeccionObservacion(obs) === seccion);
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
   * Resume quiÃ©n hizo la observaciÃ³n y cuÃ¡ndo.
   */
  resumenObservacion(obs: any): string {
    const quien = this.texto(obs?.realizadoPor || 'Director');
    const fecha = obs?.fechaObservacion ? this.formatearFechaCorta(obs.fechaObservacion) : '';
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

    if (/(descripci[oÃ³]n|problem[aÃ¡]tica|problematica)/.test(texto)) {
      return 'descripcion';
    }

    if (/(objetivo)/.test(texto)) {
      return 'objetivo';
    }

    if (/(alcance)/.test(texto)) {
      return 'alcance';
    }

    if (/(actividad|actividades|m[oÃ³]dulo|modulo|producto|productos|hora|horas|componente)/.test(texto)) {
      return 'componentes';
    }

    if (/(asignatura|participante|participantes|proyecto|t[iÃ­]tulo|titulo|datos generales|departamento|carrera)/.test(texto)) {
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
   * Renderiza marcas simples de formato usadas por el formulario.
   */
  renderizarTexto(texto: string): SafeHtml {
    if (!texto) {
      return this.sanitizer.bypassSecurityTrustHtml('<p>—</p>');
    }

    const escapado = this.escaparHtml(this.texto(texto));
    const lineas = escapado.split(/\r?\n/);
    let html = '';
    let enListaUl = false;
    let enListaOl = false;

    const cerrarListas = () => {
      if (enListaUl) {
        html += '</ul>';
        enListaUl = false;
      }
      if (enListaOl) {
        html += '</ol>';
        enListaOl = false;
      }
    };

    lineas.forEach(linea => {
      const numerada = linea.match(/^\s*\d+\.\s+(.*)$/);
      const vineta = linea.match(/^\s*-\s+(.*)$/);

      if (numerada) {
        if (!numerada[1].trim()) {
          cerrarListas();
          return;
        }

        if (enListaUl) {
          html += '</ul>';
          enListaUl = false;
        }
        if (!enListaOl) {
          html += '<ol>';
          enListaOl = true;
        }
        html += `<li>${this.aplicarMarcasInline(numerada[1])}</li>`;
        return;
      }

      if (vineta) {
        if (!vineta[1].trim()) {
          cerrarListas();
          return;
        }

        if (enListaOl) {
          html += '</ol>';
          enListaOl = false;
        }
        if (!enListaUl) {
          html += '<ul>';
          enListaUl = true;
        }
        html += `<li>${this.aplicarMarcasInline(vineta[1])}</li>`;
        return;
      }

      cerrarListas();
      html += linea.trim()
        ? `<p>${this.aplicarMarcasInline(linea)}</p>`
        : '<br>';
    });

    cerrarListas();
    return this.sanitizer.bypassSecurityTrustHtml(html);
  }

  /**
   * Escapa HTML antes de aplicar formato permitido.
   */
  private escaparHtml(texto: string): string {
    return texto
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#039;');
  }

  /**
   * Aplica negrita y cursiva en lÃ­nea.
   */
  private aplicarMarcasInline(texto: string): string {
    return texto
      .replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>')
      .replace(/(^|[^*])\*(?!\*)(.+?)\*(?!\*)/g, '$1<em>$2</em>');
  }

  /**
   * Obtiene el texto descriptivo de un estado anterior nulo
   */
  getEstadoAnterior(estadoAnterior: string | null): string {
    if (!estadoAnterior) return 'Creación';
    return this.texto(estadoAnterior);
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


