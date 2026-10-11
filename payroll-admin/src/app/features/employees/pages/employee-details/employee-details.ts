import { Component, computed, inject, input, numberAttribute } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { filter } from 'rxjs';
import { LanguageService } from '@core/i18n/language.service';
import { AbsencesApi } from '@features/absences/data-access/absences.api';
import { LocalDatePipe, MoneyPipe, PercentValuePipe } from '@shared/pipes/format.pipes';
import { PageHeader } from '@shared/ui/page-header/page-header';
import { EmptyState } from '@shared/ui/state/empty-state';
import { EmployeeFormDialog } from '../../components/employee-form-dialog/employee-form-dialog';
import { EmployeesApi } from '../../data-access/employees.api';

@Component({
  selector: 'app-employee-details',
  imports: [
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatTableModule,
    TranslatePipe,
    PageHeader,
    EmptyState,
    LocalDatePipe,
    MoneyPipe,
    PercentValuePipe,
  ],
  templateUrl: './employee-details.html',
  styleUrl: './employee-details.scss',
})
export class EmployeeDetailsPage {
  private readonly employeesApi = inject(EmployeesApi);
  private readonly absencesApi = inject(AbsencesApi);
  private readonly dialog = inject(MatDialog);
  private readonly language = inject(LanguageService);

  /** Bound from the `:id` route param. */
  readonly id = input.required({ transform: numberAttribute });

  protected readonly details = rxResource({
    params: () => ({ id: this.id(), lang: this.language.current() }),
    stream: ({ params }) => this.employeesApi.get(params.id),
  });

  protected readonly absences = rxResource({
    params: () => ({ id: this.id(), lang: this.language.current() }),
    stream: ({ params }) => this.absencesApi.list({ page: 1, pageSize: 10, employeeId: params.id }),
  });

  protected readonly info = computed(() => (this.details.hasValue() ? this.details.value() : null));
  protected readonly employee = computed(() => this.info()?.employee ?? null);
  protected readonly recentAbsences = computed(() => (this.absences.hasValue() ? this.absences.value().items : []));
  protected readonly absenceColumns = ['date', 'reason'];

  protected edit(): void {
    this.dialog
      .open(EmployeeFormDialog, { data: this.employee(), width: '720px' })
      .afterClosed()
      .pipe(filter(Boolean))
      .subscribe(() => this.details.reload());
  }
}
