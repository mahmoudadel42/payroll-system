import { Routes } from '@angular/router';

export default [
  {
    path: '',
    title: 'nav.employees',
    loadComponent: () => import('./pages/employee-list/employee-list').then((m) => m.EmployeeList),
  },
  {
    path: ':id',
    title: 'employees.details',
    loadComponent: () => import('./pages/employee-details/employee-details').then((m) => m.EmployeeDetailsPage),
  },
] satisfies Routes;
