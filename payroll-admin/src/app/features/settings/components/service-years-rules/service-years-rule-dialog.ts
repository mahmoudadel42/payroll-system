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
import { ServiceYearsRule } from '../../data-access/settings.models';
import { SettingsApi } from '../../data-access/settings.api';

@Component({
  selector: 'app-service-years-rule-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule, TranslatePipe, ControlError],
  template: `
    <h2 mat-dialog-title>{{ (rule ? 'settings.editRule' : 'settings.addRule') | translate }}</h2>
    <form [formGroup]="form" (ngSubmit)="save()" novalidate>
      <mat-dialog-content>
        <p class="muted">{{ 'settings.serviceYearsHint' | translate }}</p>
        <div class="form-grid">
          <mat-form-field>
            <mat-label>{{ 'settings.minYears' | translate }}</mat-label>
            <input matInput type="number" min="0" step="1" formControlName="minYears" />
            <mat-error><app-control-error [control]="form.controls.minYears" /></mat-error>
          </mat-form-field>
          <mat-form-field>
            <mat-label>{{ 'settings.percentage' | translate }}</mat-label>
            <input matInput type="number" min="0" max="100" step="0.01" formControlName="percentage" />
            <span matTextSuffix>%</span>
            <mat-error><app-control-error [control]="form.controls.percentage" /></mat-error>
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
export class ServiceYearsRuleDialog {
  private readonly api = inject(SettingsApi);
  private readonly dialogRef = inject(MatDialogRef<ServiceYearsRuleDialog, boolean>);
  private readonly notifications = inject(NotificationService);
  protected readonly rule = inject<ServiceYearsRule | null>(MAT_DIALOG_DATA);

  protected readonly saving = signal(false);
  protected readonly form = inject(NonNullableFormBuilder).group({
    minYears: [this.rule?.minYears ?? 0, [Validators.required, Validators.min(0), Validators.pattern(/^\d+$/)]],
    percentage: [this.rule?.percentage ?? 0, [Validators.required, Validators.min(0), Validators.max(100)]],
  });

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const request = this.form.getRawValue();
    this.saving.set(true);
    const save$: Observable<unknown> = this.rule
      ? this.api.updateServiceYearsRule(this.rule.id, request)
      : this.api.createServiceYearsRule(request);
    save$.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.notifications.success('common.savedSuccessfully');
        this.dialogRef.close(true);
      },
      error: (error: unknown) => reportFormError(this.form, error, this.notifications),
    });
  }
}
