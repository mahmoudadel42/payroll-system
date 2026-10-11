import { HttpErrorResponse } from '@angular/common/http';

/** RFC 7807 payload returned by the Payroll API (`code` is a stable machine-readable key). */
export interface ProblemDetails {
  title?: string;
  status?: number;
  code?: string;
  errors?: Record<string, string[]>;
}

export function getProblemDetails(error: unknown): ProblemDetails | null {
  if (!(error instanceof HttpErrorResponse)) return null;
  const body = error.error;
  return body && typeof body === 'object' && !(body instanceof Blob) ? (body as ProblemDetails) : null;
}

/** Best human-readable message for an API error; `null` when the server sent nothing usable. */
export function getProblemMessage(error: unknown): string | null {
  const problem = getProblemDetails(error);
  if (!problem) return null;
  const fieldErrors = Object.values(problem.errors ?? {}).flat();
  return fieldErrors.length ? fieldErrors.slice(0, 3).join('\n') : (problem.title ?? null);
}
