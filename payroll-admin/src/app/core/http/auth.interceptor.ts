import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '@core/auth/auth.service';
import { API_BASE_URL } from './api.config';
import { SKIP_AUTH } from './http-context';

const withBearer = (req: HttpRequest<unknown>, token: string) =>
  req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });

/** Attaches the access token and transparently retries once after refreshing it on a 401. */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const apiUrl = inject(API_BASE_URL);
  const token = auth.accessToken;

  if (req.context.get(SKIP_AUTH) || !req.url.startsWith(apiUrl) || !token) return next(req);

  return next(withBearer(req, token)).pipe(
    catchError((error: unknown) => {
      if (!(error instanceof HttpErrorResponse) || error.status !== 401) return throwError(() => error);

      return auth.refreshAccessToken().pipe(
        catchError(() => {
          auth.expireSession();
          return throwError(() => error);
        }),
        switchMap((freshToken) => next(withBearer(req, freshToken))),
      );
    }),
  );
};
