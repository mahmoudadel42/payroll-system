import { inject } from '@angular/core';
import { CanActivateFn, CanMatchFn, Router } from '@angular/router';
import { Role } from './auth.models';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = (_route, state) => {
  const auth = inject(AuthService);
  return auth.isAuthenticated()
    ? true
    : inject(Router).createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
};

export const guestGuard: CanActivateFn = () =>
  inject(AuthService).isAuthenticated() ? inject(Router).createUrlTree(['/']) : true;

/** Usage: `canMatch: [roleGuard('Admin')]` — users without the role are sent to the dashboard. */
export const roleGuard =
  (...roles: Role[]): CanMatchFn =>
  () =>
    inject(AuthService).hasAnyRole(roles) ? true : inject(Router).createUrlTree(['/']);
