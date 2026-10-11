import { Component, computed, effect, inject, input, numberAttribute, viewChild } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { switchMap } from 'rxjs';
import { LanguageService } from '@core/i18n/language.service';
import { NotificationService } from '@core/notifications/notification.service';
import { HasRoleDirective } from '@shared/directives/has-role.directive';
import { LocalDatePipe, MoneyPipe, PeriodPipe } from '@shared/pipes/format.pipes';
import { ConfirmService } from '@shared/ui/confirm-dialog/confirm.service';
import { PageHeader } from '@shared/ui/page-header/page-header';
import { EmptyState } from '@shared/ui/state/empty-state';
import { PayslipDialog, PayslipDialogData } from '../../components/payslip-dialog/payslip-dialog';
import { RunStatusChip } from '../../components/run-status-chip';
import { Payslip } from '../../data-access/payroll-run.models';
import { PayrollRunsApi } from '../../data-access/payroll-runs.api';

@Component({
  selector: 'app-payroll-run-details',
  imports: [
    RouterLink,
    MatTableModule,
    MatSortModule,
    MatProgressBarModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    TranslatePipe,
    PageHeader,
    EmptyState,
    RunStatusChip,
    HasRoleDirective,
    MoneyPipe,
    PeriodPipe,
    LocalDatePipe,
  ],
  templateUrl: './payroll-run-details.html',
  styleUrl: './payroll-run-details.scss',
})
export class PayrollRunDetailsPage {
  private readonly api = inject(PayrollRunsApi);
  private readonly dialog = inject(MatDialog);
  private readonly router = inject(Router);
  private readonly confirm = inject(ConfirmService);
  private readonly notifications = inject(NotificationService);
  private readonly language = inject(LanguageService);

  /** Bound from the `:id` route param. */
  readonly id = input.required({ transform: numberAttribute });

  protected readonly details = rxResource({
    params: () => ({ id: this.id(), lang: this.language.current() }),
    stream: ({ params }) => this.api.get(params.id),
  });

  protected readonly info = computed(() => (this.details.hasValue() ? this.details.value() : null));
  protected readonly run = computed(() => this.info()?.run ?? null);
  protected readonly isDraft = computed(() => this.run()?.status === 'Draft');

  protected readonly columns = [
    'employeeName',
    'departmentName',
    'grade',
    'absenceDays',
    'baseSalary',
    'incentives',
    'absenceDeduction',
    'netSalary',
  ];
  protected readonly dataSource = new MatTableDataSource<Payslip>([]);
  private readonly sort = viewChild(MatSort);

  protected readonly totals = computed(() => {
    const payslips = this.info()?.payslips ?? [];
    const sum = (pick: (p: Payslip) => number) => payslips.reduce((total, p) => total + pick(p), 0);
    return {
      baseSalary: sum((p) => p.baseSalary),
      incentives: sum(incentivesOf),
      absenceDeduction: sum((p) => p.absenceDeduction),
      netSalary: sum((p) => p.netSalary),
    };
  });

  protected readonly incentivesOf = incentivesOf;

  constructor() {
    this.dataSource.filterPredicate = (p, term) =>
      `${p.employeeName} ${p.departmentName}`.toLowerCase().includes(term);
    this.dataSource.sortingDataAccessor = (p, column) =>
      column === 'incentives' ? incentivesOf(p) : ((p as unknown as Record<string, string | number>)[column] ?? '');

    effect(() => {
      this.dataSource.data = this.info()?.payslips ?? [];
    });
    effect(() => {
      const sort = this.sort();
      if (sort) this.dataSource.sort = sort;
    });
  }

  protected applyFilter(term: string): void {
    this.dataSource.filter = term.trim().toLowerCase();
  }

  protected openPayslip(payslip: Payslip): void {
    const run = this.run()!;
    this.dialog.open<PayslipDialog, PayslipDialogData>(PayslipDialog, {
      data: { payslip, year: run.year, month: run.month },
      width: '560px',
    });
  }

  protected recalculate(): void {
    this.confirm
      .confirm({ title: 'payroll.recalculate', message: 'payroll.recalculateConfirm' })
      .pipe(switchMap(() => this.api.recalculate(this.id())))
      .subscribe(() => {
        this.notifications.success('payroll.recalculated');
        this.details.reload();
      });
  }

  protected approve(): void {
    this.confirm
      .confirm({ title: 'payroll.approve', message: 'payroll.approveConfirm', confirmText: 'payroll.approve' })
      .pipe(switchMap(() => this.api.approve(this.id())))
      .subscribe(() => {
        this.notifications.success('payroll.approved');
        this.details.reload();
      });
  }

  protected delete(): void {
    this.confirm
      .confirm({ title: 'common.deleteTitle', message: 'payroll.deleteConfirm', confirmText: 'common.delete', danger: true })
      .pipe(switchMap(() => this.api.delete(this.id())))
      .subscribe(() => {
        this.notifications.success('common.deletedSuccessfully');
        void this.router.navigate(['/payroll-runs']);
      });
  }
}

function incentivesOf(p: Payslip): number {
  return p.departmentIncentive + p.serviceYearsIncentive + p.absenceBonus;
}
