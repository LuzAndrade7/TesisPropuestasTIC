import { Routes } from '@angular/router';
import { FormularioPropuestaComponent } from './components/formulario-propuesta/formulario-propuesta.component';
import { TableroComponent } from './components/tablero/tablero.component';
import { DetallePropuestaComponent } from './components/detalle-propuesta/detalle-propuesta.component';

/**
 * Rutas principales de la aplicación
 * T03: Integración de componentes
 * T06: Tablero HU02
 * HU06 T19: Detalle de propuesta
 */
export const routes: Routes = [
  {
    path: '',
    redirectTo: 'tablero',
    pathMatch: 'full'
  },
  {
    path: 'tablero',
    component: TableroComponent
  },
  {
    path: 'propuestas/nueva',
    component: FormularioPropuestaComponent
  },
  {
    path: 'propuestas/:id',
    component: FormularioPropuestaComponent
  },
  {
    path: 'propuestas/:id/editar',
    component: FormularioPropuestaComponent
  },
  {
    path: 'propuestas/:id/detalle',
    component: DetallePropuestaComponent
  }
];
