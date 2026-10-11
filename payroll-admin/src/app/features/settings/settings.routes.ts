import { Routes } from '@angular/router';

export default [
  {
    path: '',
    title: 'nav.settings',
    loadComponent: () => import('./pages/settings-page/settings-page').then((m) => m.SettingsPage),
  },
] satisfies Routes;
