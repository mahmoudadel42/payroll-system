import { Routes } from '@angular/router';

export default [
  {
    path: '',
    title: 'nav.departments',
    loadComponent: () => import('./pages/department-list/department-list').then((m) => m.DepartmentList),
  },
] satisfies Routes;
