import { Routes } from '@angular/router';

export default [
  {
    path: '',
    title: 'nav.payrollRuns',
    loadComponent: () => import('./pages/payroll-run-list/payroll-run-list').then((m) => m.PayrollRunList),
  },
  {
    path: ':id',
    title: 'payroll.details',
    loadComponent: () => import('./pages/payroll-run-details/payroll-run-details').then((m) => m.PayrollRunDetailsPage),
  },
] satisfies Routes;
