import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '@core/notifications/notification.service';
import { SKIP_ERROR_TOAST } from './http-context';
import { getProblemMessage } from './problem-details';

/** Shows a toast for failed requests. 401s are handled by the auth interceptor. */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notifications = inject(NotificationService);

  return next(req).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse && error.status !== 401 && !req.context.get(SKIP_ERROR_TOAST)) {
        notifications.error(messageFor(error));
      }
      return throwError(() => error);
    }),
  );
};

function messageFor(error: HttpErrorResponse): string {
  if (error.status === 0) return 'errors.network';
  if (error.status === 403) return 'errors.forbidden';
  return getProblemMessage(error) ?? (error.status >= 500 ? 'errors.server' : 'errors.generic');
}
