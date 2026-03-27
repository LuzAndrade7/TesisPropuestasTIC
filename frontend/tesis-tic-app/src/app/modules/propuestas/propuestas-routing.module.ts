import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListaPropuestasComponent } from './components/lista-propuestas.component';
import { CrearPropuestaComponent } from './components/crear-propuesta.component';
import { DetallePropuestaComponent } from './components/detalle-propuesta.component';

const routes: Routes = [
  {
    path: '',
    component: ListaPropuestasComponent
  },
  {
    path: 'nueva',
    component: CrearPropuestaComponent
  },
  {
    path: ':id',
    component: DetallePropuestaComponent
  },
  {
    path: ':id/editar',
    component: CrearPropuestaComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PropuestasRoutingModule { }
