import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { filter } from 'rxjs';
import { pagedResource } from '@shared/data-access/paged-resource';
import { DEFAULT_PAGE_SIZE, PAGE_SIZE_OPTIONS, PAYROLL_RUN_STATUSES, PayrollRunStatus } from '@shared/models/api.models';
import { LocalDatePipe, MoneyPipe, PeriodPipe } from '@shared/pipes/format.pipes';
import { PageHeader } from '@shared/ui/page-header/page-header';
import { EmptyState } from '@shared/ui/state/empty-state';
import { recentYears } from '@shared/utils/date';
import { GenerateRunDialog } from '../../components/generate-run-dialog/generate-run-dialog';
import { RunStatusChip } from '../../components/run-status-chip';
import { PayrollRun, PayrollRunsQuery } from '../../data-access/payroll-run.models';
import { PayrollRunsApi } from '../../data-access/payroll-runs.api';

@Component({
  selector: 'app-payroll-run-list',
  imports: [
    MatTableModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    TranslatePipe,
    PageHeader,
    EmptyState,
    RunStatusChip,
    MoneyPipe,
    PeriodPipe,
    LocalDatePipe,
  ],
  templateUrl: './payroll-run-list.html',
})
export class PayrollRunList {
  private readonly api = inject(PayrollRunsApi);
  private readonly dialog = inject(MatDialog);
  private readonly router = inject(Router);

  protected readonly years = recentYears();
  protected readonly statuses = PAYROLL_RUN_STATUSES;
  protected readonly pageSizeOptions = PAGE_SIZE_OPTIONS;
  protected readonly columns = ['period', 'status', 'employeesCount', 'totalNet', 'createdAt', 'approvedAt', 'open'];

  protected readonly list = pagedResource<PayrollRun, PayrollRunsQuery>({
    initialQuery: { page: 1, pageSize: DEFAULT_PAGE_SIZE, year: null, status: null },
    loader: (query) => this.api.list(query),
  });

  protected filterByYear(year: number | null): void {
    this.list.filter({ year });
  }

  protected filterByStatus(status: PayrollRunStatus | null): void {
    this.list.filter({ status });
  }

  protected open(run: PayrollRun): void {
    void this.router.navigate(['/payroll-runs', run.id]);
  }

  protected generate(): void {
    this.dialog
      .open(GenerateRunDialog, { width: '480px' })
      .afterClosed()
      .pipe(filter((id): id is number => typeof id === 'number'))
      .subscribe((id) => void this.router.navigate(['/payroll-runs', id]));
  }
}
