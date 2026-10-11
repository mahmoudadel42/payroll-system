import { Role } from '@core/auth/auth.models';

export interface NavItem {
  /** Translation key. */
  label: string;
  icon: string;
  route: string;
  /** Empty means every signed-in user. */
  roles: Role[];
}

export const NAV_ITEMS: readonly NavItem[] = [
  { label: 'nav.dashboard', icon: 'dashboard', route: '/dashboard', roles: [] },
  { label: 'nav.employees', icon: 'badge', route: '/employees', roles: [] },
  { label: 'nav.departments', icon: 'apartment', route: '/departments', roles: [] },
  { label: 'nav.absences', icon: 'event_busy', route: '/absences', roles: [] },
  { label: 'nav.payrollRuns', icon: 'payments', route: '/payroll-runs', roles: [] },
  { label: 'nav.reports', icon: 'insights', route: '/reports', roles: [] },
  { label: 'nav.settings', icon: 'tune', route: '/settings', roles: [] },
];
