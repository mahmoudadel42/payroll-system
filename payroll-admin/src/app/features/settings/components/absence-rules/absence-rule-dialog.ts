import { Component, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { TranslatePipe } from '@ngx-translate/core';
import { Observable, finalize } from 'rxjs';
import { NotificationService } from '@core/notifications/notification.service';
import { ControlError } from '@shared/forms/control-error';
import { reportFormError } from '@shared/forms/server-errors';
import { ABSENCE_RULE_TYPES, AbsenceRuleType } from '@shared/models/api.models';
import { AbsenceRule } from '../../data-access/settings.models';
import { SettingsApi } from '../../data-access/settings.api';

@Component({
  selector: 'app-absence-rule-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    TranslatePipe,
    ControlError,
  ],
  template: `
    <h2 mat-dialog-title>{{ (rule ? 'settings.editRule' : 'settings.addRule') | translate }}</h2>
    <form [formGroup]="form" (ngSubmit)="save()" novalidate>
      <mat-dialog-content>
        <div class="form-grid">
          <mat-form-field class="full-width">
            <mat-label>{{ 'settings.ruleType' | translate }}</mat-label>
            <mat-select formControlName="type">
              @for (type of types; track type) {
                <mat-option [value]="type">{{ 'enums.absenceRuleType.' + type | translate }}</mat-option>
              }
            </mat-select>
            <mat-hint>{{ 'settings.absenceRuleHint.' + type() | translate }}</mat-hint>
          </mat-form-field>
          <mat-form-field>
            <mat-label>{{ 'settings.thresholdDays' | translate }}</mat-label>
            <input matInput type="number" min="0" step="1" formControlName="thresholdDays" />
            <mat-error><app-control-error [control]="form.controls.thresholdDays" /></mat-error>
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
export class AbsenceRuleDialog {
  private readonly api = inject(SettingsApi);
  private readonly dialogRef = inject(MatDialogRef<AbsenceRuleDialog, boolean>);
  private readonly notifications = inject(NotificationService);
  protected readonly rule = inject<AbsenceRule | null>(MAT_DIALOG_DATA);

  protected readonly types = ABSENCE_RULE_TYPES;
  protected readonly saving = signal(false);
  protected readonly form = inject(NonNullableFormBuilder).group({
    type: [this.rule?.type ?? ('Deduction' as AbsenceRuleType), Validators.required],
    thresholdDays: [this.rule?.thresholdDays ?? 0, [Validators.required, Validators.min(0), Validators.pattern(/^\d+$/)]],
    percentage: [this.rule?.percentage ?? 0, [Validators.required, Validators.min(0), Validators.max(100)]],
  });
  protected readonly type = toSignal(this.form.controls.type.valueChanges, {
    initialValue: this.form.controls.type.value,
  });

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const request = this.form.getRawValue();
    this.saving.set(true);
    const save$: Observable<unknown> = this.rule ? this.api.updateAbsenceRule(this.rule.id, request) : this.api.createAbsenceRule(request);
    save$.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.notifications.success('common.savedSuccessfully');
        this.dialogRef.close(true);
      },
      error: (error: unknown) => reportFormError(this.form, error, this.notifications),
    });
  }
}
