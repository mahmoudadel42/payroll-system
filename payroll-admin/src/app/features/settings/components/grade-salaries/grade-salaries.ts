import { Component, computed, inject, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslatePipe } from '@ngx-translate/core';
import { finalize } from 'rxjs';
import { getProblemMessage } from '@core/http/problem-details';
import { NotificationService } from '@core/notifications/notification.service';
import { ControlError } from '@shared/forms/control-error';
import { HasRoleDirective } from '@shared/directives/has-role.directive';
import { JobGrade } from '@shared/models/api.models';
import { LocalDatePipe, MoneyPipe } from '@shared/pipes/format.pipes';
import { GradeSalary } from '../../data-access/settings.models';
import { SettingsApi } from '../../data-access/settings.api';

/** Base salary per job grade, edited inline one row at a time. */
@Component({
  selector: 'app-grade-salaries',
  imports: [
    ReactiveFormsModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatProgressBarModule,
    TranslatePipe,
    HasRoleDirective,
    ControlError,
    MoneyPipe,
    LocalDatePipe,
  ],
  templateUrl: './grade-salaries.html',
})
export class GradeSalaries {
  private readonly api = inject(SettingsApi);
  private readonly notifications = inject(NotificationService);

  protected readonly resource = rxResource({ stream: () => this.api.gradeSalaries() });
  protected readonly rows = computed(() => (this.resource.hasValue() ? this.resource.value() : []));
  protected readonly columns = ['grade', 'baseSalary', 'updatedAt', 'actions'];

  protected readonly editing = signal<JobGrade | null>(null);
  protected readonly saving = signal(false);
  protected readonly salary = new FormControl<number | null>(null, [Validators.required, Validators.min(0.01)]);

  protected startEdit(row: GradeSalary): void {
    this.salary.reset(row.baseSalary);
    this.editing.set(row.grade);
  }

  protected cancel(): void {
    this.editing.set(null);
  }

  protected save(row: GradeSalary): void {
    if (this.salary.invalid) {
      this.salary.markAsTouched();
      return;
    }
    this.saving.set(true);
    this.api
      .saveGradeSalary(row, this.salary.value!)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          this.notifications.success('common.savedSuccessfully');
          this.editing.set(null);
          this.resource.reload();
        },
        error: (error: unknown) => this.notifications.error(getProblemMessage(error) ?? 'errors.generic'),
      });
  }
}
