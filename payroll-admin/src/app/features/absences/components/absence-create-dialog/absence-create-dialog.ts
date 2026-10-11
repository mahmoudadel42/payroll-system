import { Component, computed, inject, signal } from '@angular/core';
import { FormControl, NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatDatepickerInputEvent, MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TranslatePipe } from '@ngx-translate/core';
import { finalize } from 'rxjs';
import { NotificationService } from '@core/notifications/notification.service';
import { EmployeePicker } from '@features/employees/components/employee-picker/employee-picker';
import { ControlError } from '@shared/forms/control-error';
import { reportFormError } from '@shared/forms/server-errors';
import { Lookup } from '@shared/models/api.models';
import { LocalDatePipe } from '@shared/pipes/format.pipes';
import { toIsoDate, today } from '@shared/utils/date';
import { AbsencesApi } from '../../data-access/absences.api';

/** Records one or more absence days for a single employee. */
@Component({
  selector: 'app-absence-create-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatChipsModule,
    MatIconModule,
    MatButtonModule,
    TranslatePipe,
    ControlError,
    LocalDatePipe,
    EmployeePicker,
  ],
  templateUrl: './absence-create-dialog.html',
  styles: `
    .dates {
      grid-column: 1 / -1;
      min-height: 40px;
    }
    .dates__hint {
      margin: 0;
      font: var(--mat-sys-body-small);
    }
  `,
})
export class AbsenceCreateDialog {
  private readonly api = inject(AbsencesApi);
  private readonly dialogRef = inject(MatDialogRef<AbsenceCreateDialog, boolean>);
  private readonly notifications = inject(NotificationService);

  protected readonly maxDate = today();
  protected readonly saving = signal(false);
  protected readonly dates = signal<string[]>([]);
  protected readonly datesTouched = signal(false);
  protected readonly datesInvalid = computed(() => this.datesTouched() && this.dates().length === 0);

  protected readonly form = inject(NonNullableFormBuilder).group({
    employee: new FormControl<Lookup | null>(null, Validators.required),
    reason: ['', Validators.maxLength(500)],
  });

  protected readonly pickerControl = new FormControl<Date | null>(null);

  protected addDate(event: MatDatepickerInputEvent<Date>): void {
    const iso = toIsoDate(event.value);
    if (iso && !this.dates().includes(iso)) {
      this.dates.update((dates) => [...dates, iso].sort());
    }
    this.datesTouched.set(true);
    this.pickerControl.setValue(null);
  }

  protected removeDate(date: string): void {
    this.dates.update((dates) => dates.filter((d) => d !== date));
  }

  /** Highlights already-selected days in the calendar. */
  protected readonly dateClass = (date: Date): string =>
    this.dates().includes(toIsoDate(date)!) ? 'selected-absence-day' : '';

  protected save(): void {
    this.datesTouched.set(true);
    if (this.form.invalid || this.dates().length === 0) {
      this.form.markAllAsTouched();
      return;
    }
    const { employee, reason } = this.form.getRawValue();
    this.saving.set(true);
    this.api
      .createMany({ employeeId: employee!.id, dates: this.dates(), reason: reason.trim() || null })
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: (created) => {
          this.notifications.success('absences.created', { count: created });
          this.dialogRef.close(true);
        },
        error: (error: unknown) => reportFormError(this.form, error, this.notifications),
      });
  }
}
