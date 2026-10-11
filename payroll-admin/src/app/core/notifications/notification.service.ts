import { Injectable, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly snackBar = inject(MatSnackBar);
  private readonly translate = inject(TranslateService);

  /** Shows a translated success message. */
  success(key: string, params?: Record<string, unknown>): void {
    this.open(this.translate.instant(key, params), 'snack-success', 3000);
  }

  /** Shows an error; `message` is either a translation key or an already-localized server message. */
  error(message: string): void {
    this.open(this.translate.instant(message), 'snack-error', 6000);
  }

  private open(message: string, panelClass: string, duration: number): void {
    this.snackBar.open(message, this.translate.instant('common.close'), {
      duration,
      panelClass,
      horizontalPosition: 'end',
      verticalPosition: 'bottom',
    });
  }
}
