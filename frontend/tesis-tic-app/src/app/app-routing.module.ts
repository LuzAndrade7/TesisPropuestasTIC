import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { CrearPropuestaComponent } from './components/crear-propuesta/crear-propuesta.component';
import { DetallePropuestaComponent } from './components/detalle-propuesta/detalle-propuesta.component';
import { AsignarEstudiantesComponent } from './components/asignar-estudiantes/asignar-estudiantes.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'crear-propuesta', component: CrearPropuestaComponent },
  { path: 'editar/:id', component: CrearPropuestaComponent },
  { path: 'detalle/:id', component: DetallePropuestaComponent },
  { path: 'asignar/:id', component: AsignarEstudiantesComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    enableTracing: false,
    useHash: false
  })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
