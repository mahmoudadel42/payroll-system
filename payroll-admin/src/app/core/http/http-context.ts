import { HttpContext, HttpContextToken } from '@angular/common/http';

/** Request is sent without the bearer token and never triggers a token refresh. */
export const SKIP_AUTH = new HttpContextToken<boolean>(() => false);

/** The caller renders the error itself, so the global error toast is suppressed. */
export const SKIP_ERROR_TOAST = new HttpContextToken<boolean>(() => false);

export const silentErrors = () => new HttpContext().set(SKIP_ERROR_TOAST, true);
