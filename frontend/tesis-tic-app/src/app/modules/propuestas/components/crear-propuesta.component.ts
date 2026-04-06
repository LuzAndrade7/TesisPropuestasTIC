import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { PropuestaService } from '../services/propuesta.service';
import {
  DocenteInfo,
  LineaInvestigacion,
  Asignatura,
  CrearPropuestaRequest,
  ActualizarPropuestaRequest,
  Propuesta
} from '../../../shared/models/propuesta.model';

@Component({
  selector: 'app-crear-propuesta',
  templateUrl: './crear-propuesta.component.html',
  styleUrls: ['./crear-propuesta.component.css']
})
export class CrearPropuestaComponent implements OnInit {
  formularioPropuesta: FormGroup;
  docentes: DocenteInfo[] = [];
  lineasInvestigacion: LineaInvestigacion[] = [];
  asignaturas: Asignatura[] = [];
  asignaturasSeleccionadas: number[] = [];

  enviando: boolean = false;
  error: string = '';
  exito: boolean = false;

  esEdicion: boolean = false;
  propuestaId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private propuestaService: PropuestaService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.formularioPropuesta = this.crearFormulario();
  }

  ngOnInit(): void {
    this.cargarDatos();

    this.route.params.subscribe((params: any) => {
      if (params['id']) {
        this.esEdicion = true;
        this.propuestaId = Number(params['id']);
        this.cargarPropuesta(this.propuestaId);
      }
    });
  }

  crearFormulario(): FormGroup {
    return this.fb.group({
      titulo: ['', [Validators.required, Validators.minLength(10)]],
      descripcion: ['', [Validators.required, Validators.minLength(50)]],
      objetivo: ['', [Validators.required, Validators.minLength(30)]],
      alcance: ['', [Validators.required, Validators.minLength(30)]],
      componentesActividadesProductos: ['', [Validators.required, Validators.minLength(50)]],
      docenteId: ['', Validators.required],
      lineaInvestigacionId: [''],
      numeroParticipantes: ['', [Validators.required, Validators.min(1)]],
      departamento: ['', Validators.required],
      facultad: ['', Validators.required]
    });
  }

  cargarDatos(): void {
    this.propuestaService.obtenerDocentes().subscribe({
      next: (docentes) => this.docentes = docentes
    });

    this.propuestaService.obtenerAsignaturas().subscribe({
      next: (asignaturas) => this.asignaturas = asignaturas
    });
  }

  cargarPropuesta(id: number): void {
    this.propuestaService.obtenerPropuesta(id).subscribe({
      next: (propuesta) => {
        this.llenarFormularioPropuesta(propuesta);
      },
      error: () => {
        this.error = 'Error al cargar la propuesta';
      }
    });
  }

  llenarFormularioPropuesta(propuesta: Propuesta): void {
    this.formularioPropuesta.patchValue({
      titulo: propuesta.titulo,
      descripcion: propuesta.descripcion,
      objetivo: propuesta.objetivo,
      alcance: propuesta.alcance,
      componentesActividadesProductos: propuesta.componentesActividadesProductos,
      docenteId: propuesta.docenteId,
      lineaInvestigacionId: propuesta.lineaInvestigacionId,
      numeroParticipantes: propuesta.numeroParticipantes,
      departamento: propuesta.departamento,
      facultad: propuesta.facultad
    });
    this.asignaturasSeleccionadas = propuesta.asignaturas.map(a => a.id);
  }

  seleccionarAsignatura(event: any, asignaturaId: number): void {
    if (event.target.checked) {
      this.asignaturasSeleccionadas.push(asignaturaId);
    } else {
      this.asignaturasSeleccionadas = this.asignaturasSeleccionadas.filter(
        (id: number) => id !== asignaturaId
      );
    }
  }

  estaAsignaturaSeleccionada(asignaturaId: number): boolean {
    return this.asignaturasSeleccionadas.includes(asignaturaId);
  }

  enviar(): void {
    if (this.formularioPropuesta.invalid) {
      this.validarFormulario();
      return;
    }

    this.enviando = true;
    this.error = '';

    if (this.esEdicion && this.propuestaId) {
      this.actualizarPropuesta();
    } else {
      this.crearPropuesta();
    }
  }

  crearPropuesta(): void {
    const request: CrearPropuestaRequest = {
      ...this.formularioPropuesta.value,
      asignaturasIds: this.asignaturasSeleccionadas
    };

    this.propuestaService.crearPropuesta(request).subscribe({
      next: (respuesta) => {
        this.enviando = false;
        this.exito = true;
        setTimeout(() => {
          this.router.navigate(['/propuestas']);
        }, 1500);
      },
      error: (error) => {
        this.enviando = false;
        this.error = error.error?.message || 'Error al crear la propuesta';
      }
    });
  }

  actualizarPropuesta(): void {
    const request: ActualizarPropuestaRequest = {
      id: this.propuestaId!,
      ...this.formularioPropuesta.value,
      asignaturasIds: this.asignaturasSeleccionadas,
      observaciones: ''
    };

    this.propuestaService.actualizarPropuesta(request).subscribe({
      next: () => {
        this.enviando = false;
        this.exito = true;
        setTimeout(() => {
          this.router.navigate(['/propuestas']);
        }, 1500);
      },
      error: (error) => {
        this.enviando = false;
        this.error = error.error?.message || 'Error al actualizar la propuesta';
      }
    });
  }

  validarFormulario(): void {
    Object.keys(this.formularioPropuesta.controls).forEach(key => {
      const control = this.formularioPropuesta.get(key);
      if (control && control.invalid) {
        control.markAsTouched();
      }
    });
  }

  obtenerErrorCampo(nombreCampo: string): string {
    const control = this.formularioPropuesta.get(nombreCampo);
    if (!control || !control.errors || !control.touched) return '';

    if (control.errors['required']) return 'Este campo es requerido';
    if (control.errors['minlength']) {
      const minLength = control.errors['minlength'].requiredLength;
      return `Mínimo ${minLength} caracteres`;
    }
    if (control.errors['min']) return 'Debe ser mayor a 0';

    return 'Error de validación';
  }

  volver(): void {
    this.router.navigate(['/propuestas']);
  }
}
