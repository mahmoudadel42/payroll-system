import { Routes } from '@angular/router';

export default [
  {
    path: '',
    title: 'nav.absences',
    loadComponent: () => import('./pages/absence-list/absence-list').then((m) => m.AbsenceList),
  },
] satisfies Routes;
