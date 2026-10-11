import { HttpErrorResponse } from '@angular/common/http';
import { FormGroup } from '@angular/forms';
import { getProblemDetails, getProblemMessage } from '@core/http/problem-details';
import { NotificationService } from '@core/notifications/notification.service';

/** Attaches field errors to the form, and toasts anything that could not be tied to a field. */
export function reportFormError(form: FormGroup, error: unknown, notifications: NotificationService): void {
  if (error instanceof HttpErrorResponse && error.status === 401) return;
  if (applyServerErrors(form, error)) return;
  notifications.error(getProblemMessage(error) ?? 'errors.generic');
}

/**
 * Maps API validation errors (`errors: { fieldName: [...] }`) onto matching form controls.
 * Returns true when at least one error was attached to a control.
 */
export function applyServerErrors(form: FormGroup, error: unknown): boolean {
  const errors = getProblemDetails(error)?.errors;
  if (!errors) return false;

  let applied = false;
  for (const [field, messages] of Object.entries(errors)) {
    const control = form.get(field);
    if (control && messages.length) {
      control.setErrors({ server: messages[0] });
      control.markAsTouched();
      applied = true;
    }
  }
  return applied;
}
