import { Routes } from '@angular/router';
import { authGuard, guestGuard } from '@core/auth/auth.guards';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    title: 'auth.login',
    loadComponent: () => import('@features/auth/login/login').then((m) => m.Login),
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('@core/layout/shell/shell').then((m) => m.Shell),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      {
        path: 'dashboard',
        title: 'nav.dashboard',
        loadComponent: () => import('@features/dashboard/dashboard').then((m) => m.Dashboard),
      },
      { path: 'employees', loadChildren: () => import('@features/employees/employees.routes') },
      { path: 'departments', loadChildren: () => import('@features/departments/departments.routes') },
      { path: 'absences', loadChildren: () => import('@features/absences/absences.routes') },
      { path: 'payroll-runs', loadChildren: () => import('@features/payroll-runs/payroll-runs.routes') },
      { path: 'reports', loadChildren: () => import('@features/reports/reports.routes') },
      { path: 'settings', loadChildren: () => import('@features/settings/settings.routes') },
    ],
  },
  { path: '**', redirectTo: '' },
];
