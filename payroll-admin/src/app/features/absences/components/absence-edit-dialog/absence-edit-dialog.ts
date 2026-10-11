import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslatePipe } from '@ngx-translate/core';
import { finalize } from 'rxjs';
import { NotificationService } from '@core/notifications/notification.service';
import { ControlError } from '@shared/forms/control-error';
import { reportFormError } from '@shared/forms/server-errors';
import { fromIsoDate, toIsoDate, today } from '@shared/utils/date';
import { Absence } from '../../data-access/absence.models';
import { AbsencesApi } from '../../data-access/absences.api';

@Component({
  selector: 'app-absence-edit-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatButtonModule,
    TranslatePipe,
    ControlError,
  ],
  template: `
    <h2 mat-dialog-title>{{ 'absences.edit' | translate }}</h2>
    <form [formGroup]="form" (ngSubmit)="save()" novalidate>
      <mat-dialog-content>
        <p class="muted">{{ absence.employeeName }} · {{ absence.departmentName }}</p>
        <div class="form-grid">
          <mat-form-field class="full-width">
            <mat-label>{{ 'absences.date' | translate }}</mat-label>
            <input matInput [matDatepicker]="picker" formControlName="date" [max]="maxDate" />
            <mat-datepicker-toggle matIconSuffix [for]="picker" />
            <mat-datepicker #picker />
            <mat-error><app-control-error [control]="form.controls.date" /></mat-error>
          </mat-form-field>
          <mat-form-field class="full-width">
            <mat-label>{{ 'absences.reason' | translate }}</mat-label>
            <textarea matInput formControlName="reason" rows="2"></textarea>
            <mat-error><app-control-error [control]="form.controls.reason" /></mat-error>
          </mat-form-field>
        </div>
      </mat-dialog-content>
      <mat-dialog-actions align="end">
        <button mat-button type="button" mat-dialog-close>{{ 'common.cancel' | translate }}</button>
        <button mat-flat-button type="submit" [disabled]="saving()">{{ 'common.save' | translate }}</button>
      </mat-dialog-actions>
    </form>
  `,
})
export class AbsenceEditDialog {
  private readonly api = inject(AbsencesApi);
  private readonly dialogRef = inject(MatDialogRef<AbsenceEditDialog, boolean>);
  private readonly notifications = inject(NotificationService);
  protected readonly absence = inject<Absence>(MAT_DIALOG_DATA);

  protected readonly maxDate = today();
  protected readonly saving = signal(false);

  private readonly fb = inject(NonNullableFormBuilder);
  protected readonly form = this.fb.group({
    date: this.fb.control<Date | null>(fromIsoDate(this.absence.date), Validators.required),
    reason: [this.absence.reason ?? '', Validators.maxLength(500)],
  });

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const { date, reason } = this.form.getRawValue();
    this.saving.set(true);
    this.api
      .update(this.absence.id, { date: toIsoDate(date)!, reason: reason.trim() || null })
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          this.notifications.success('common.savedSuccessfully');
          this.dialogRef.close(true);
        },
        error: (error: unknown) => reportFormError(this.form, error, this.notifications),
      });
  }
}
