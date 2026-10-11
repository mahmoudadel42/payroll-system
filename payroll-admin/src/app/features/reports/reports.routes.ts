import { Routes } from '@angular/router';

export default [
  {
    path: '',
    title: 'nav.reports',
    loadComponent: () => import('./pages/reports-page/reports-page').then((m) => m.ReportsPage),
  },
] satisfies Routes;
