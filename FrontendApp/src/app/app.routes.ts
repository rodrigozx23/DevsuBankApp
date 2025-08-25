import { Routes } from '@angular/router';
export const routes: Routes = [
  { path: '', redirectTo: 'clientes', pathMatch: 'full' },
  // temporal: páginas vacías
  { path: '', redirectTo: 'clientes', pathMatch: 'full' },
  { path: 'clientes', loadComponent: () => import('./features/clientes/clientes-list.component').then(m => m.ClientesListComponent) },
  { path: 'clientes/nuevo', loadComponent: () => import('./features/clientes/cliente-form.component').then(m => m.ClienteFormComponent) },
  { path: 'clientes/:id', loadComponent: () => import('./features/clientes/cliente-form.component').then(m => m.ClienteFormComponent) },
  { path: 'cuentas', loadComponent: () => import('./features/cuentas/cuentas-list.component').then(m => m.CuentasListComponent) },
  { path: 'cuentas/nueva', loadComponent: () => import('./features/cuentas/cuenta-form.component').then(m => m.CuentaFormComponent) },
  { path: 'cuentas/:id', loadComponent: () => import('./features/cuentas/cuenta-form.component').then(m => m.CuentaFormComponent) },
  { path: 'movimientos',            loadComponent: () => import('./features/movimientos/movimientos-list.component').then(m => m.MovimientosListComponent) },
  { path: 'movimientos/nuevo',      loadComponent: () => import('./features/movimientos/movimiento-form.component').then(m => m.MovimientoFormComponent) },
  { path: 'reportes/estado-cuenta', loadComponent: () => import('./features/reportes/reporte-estado.component').then(m => m.ReporteEstadoComponent) },
  { path: 'reportes', redirectTo: 'reportes/estado-cuenta', pathMatch: 'full' },
];
