import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslatePipe } from '@ngx-translate/core';
import { Observable, finalize } from 'rxjs';
import { NotificationService } from '@core/notifications/notification.service';
import { ControlError } from '@shared/forms/control-error';
import { reportFormError } from '@shared/forms/server-errors';
import { Department } from '../../data-access/department.models';
import { DepartmentsApi } from '../../data-access/departments.api';

@Component({
  selector: 'app-department-form-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule, TranslatePipe, ControlError],
  template: `
    <h2 mat-dialog-title>{{ (department ? 'departments.edit' : 'departments.create') | translate }}</h2>
    <form [formGroup]="form" (ngSubmit)="save()" novalidate>
      <mat-dialog-content>
        <div class="form-grid">
          <mat-form-field class="full-width">
            <mat-label>{{ 'departments.nameEn' | translate }}</mat-label>
            <input matInput formControlName="nameEn" dir="ltr" />
            <mat-error><app-control-error [control]="form.controls.nameEn" /></mat-error>
          </mat-form-field>
          <mat-form-field class="full-width">
            <mat-label>{{ 'departments.nameAr' | translate }}</mat-label>
            <input matInput formControlName="nameAr" dir="rtl" />
            <mat-error><app-control-error [control]="form.controls.nameAr" /></mat-error>
          </mat-form-field>
          <mat-form-field class="full-width">
            <mat-label>{{ 'departments.incentivePercentage' | translate }}</mat-label>
            <input matInput type="number" formControlName="incentivePercentage" min="0" max="100" step="0.01" />
            <span matTextSuffix>%</span>
            <mat-error><app-control-error [control]="form.controls.incentivePercentage" /></mat-error>
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
export class DepartmentFormDialog {
  private readonly api = inject(DepartmentsApi);
  private readonly dialogRef = inject(MatDialogRef<DepartmentFormDialog, boolean>);
  private readonly notifications = inject(NotificationService);
  protected readonly department = inject<Department | null>(MAT_DIALOG_DATA);

  protected readonly saving = signal(false);
  protected readonly form = inject(NonNullableFormBuilder).group({
    nameEn: [this.department?.nameEn ?? '', [Validators.required, Validators.maxLength(150)]],
    nameAr: [this.department?.nameAr ?? '', [Validators.required, Validators.maxLength(150)]],
    incentivePercentage: [
      this.department?.incentivePercentage ?? 0,
      [Validators.required, Validators.min(0), Validators.max(100)],
    ],
  });

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const request = this.form.getRawValue();
    const save$: Observable<unknown> = this.department ? this.api.update(this.department.id, request) : this.api.create(request);

    save$.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.notifications.success('common.savedSuccessfully');
        this.dialogRef.close(true);
      },
      error: (error: unknown) => reportFormError(this.form, error, this.notifications),
    });
  }
}
