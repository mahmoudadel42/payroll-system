import { Component, computed, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { TranslatePipe } from '@ngx-translate/core';
import { finalize } from 'rxjs';
import { LanguageService } from '@core/i18n/language.service';
import { NotificationService } from '@core/notifications/notification.service';
import { reportFormError } from '@shared/forms/server-errors';
import { monthNames, recentYears } from '@shared/utils/date';
import { PayrollRunsApi } from '../../data-access/payroll-runs.api';

/** Closes with the new run id on success. */
@Component({
  selector: 'app-generate-run-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatSelectModule, MatButtonModule, TranslatePipe],
  template: `
    <h2 mat-dialog-title>{{ 'payroll.generate' | translate }}</h2>
    <form [formGroup]="form" (ngSubmit)="generate()" novalidate>
      <mat-dialog-content>
        <p class="muted">{{ 'payroll.generateHint' | translate }}</p>
        <div class="form-grid">
          <mat-form-field>
            <mat-label>{{ 'common.month' | translate }}</mat-label>
            <mat-select formControlName="month">
              @for (name of months(); track $index) {
                <mat-option [value]="$index + 1">{{ name }}</mat-option>
              }
            </mat-select>
          </mat-form-field>
          <mat-form-field>
            <mat-label>{{ 'common.year' | translate }}</mat-label>
            <mat-select formControlName="year">
              @for (year of years; track year) {
                <mat-option [value]="year">{{ year }}</mat-option>
              }
            </mat-select>
          </mat-form-field>
        </div>
      </mat-dialog-content>
      <mat-dialog-actions align="end">
        <button mat-button type="button" mat-dialog-close>{{ 'common.cancel' | translate }}</button>
        <button mat-flat-button type="submit" [disabled]="saving()">{{ 'payroll.generate' | translate }}</button>
      </mat-dialog-actions>
    </form>
  `,
})
export class GenerateRunDialog {
  private readonly api = inject(PayrollRunsApi);
  private readonly dialogRef = inject(MatDialogRef<GenerateRunDialog, number>);
  private readonly notifications = inject(NotificationService);
  private readonly language = inject(LanguageService);

  protected readonly years = recentYears();
  protected readonly months = computed(() => monthNames(this.language.locale()));
  protected readonly saving = signal(false);

  private readonly now = new Date();
  protected readonly form = inject(NonNullableFormBuilder).group({
    year: [this.now.getFullYear(), Validators.required],
    month: [this.now.getMonth() + 1, Validators.required],
  });

  protected generate(): void {
    this.saving.set(true);
    this.api
      .generate(this.form.getRawValue())
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: (id) => {
          this.notifications.success('payroll.generated');
          this.dialogRef.close(id);
        },
        error: (error: unknown) => reportFormError(this.form, error, this.notifications),
      });
  }
}
