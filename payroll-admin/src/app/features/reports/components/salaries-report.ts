import { Component, computed, inject, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { TranslatePipe } from '@ngx-translate/core';
import { LanguageService } from '@core/i18n/language.service';
import { MoneyPipe } from '@shared/pipes/format.pipes';
import { EmptyState } from '@shared/ui/state/empty-state';
import { injectReportExport } from '../data-access/report-export';
import { MonthlyReportFilter, currentMonthFilter } from '../data-access/report.models';
import { ReportsApi } from '../data-access/reports.api';
import { MonthlyFilterBar } from './monthly-filter-bar';
import { ReportStatusBanner } from './report-status-banner';

@Component({
  selector: 'app-salaries-report',
  imports: [MatTableModule, MatProgressBarModule, TranslatePipe, MoneyPipe, EmptyState, MonthlyFilterBar, ReportStatusBanner],
  template: `
    <app-monthly-filter-bar [(filter)]="filter" [exporting]="exporting()" (export)="export(filter())" />
    @if (report(); as report) {
      <app-report-status-banner [isPreview]="report.isPreview" [runStatus]="report.runStatus" />
    }
    <div class="table-wrapper">
      @if (resource.isLoading()) {
        <mat-progress-bar class="loading-bar" mode="indeterminate" />
      }
      <table mat-table [dataSource]="report()?.rows ?? []">
        <ng-container matColumnDef="employeeName">
          <th mat-header-cell *matHeaderCellDef>{{ 'absences.employee' | translate }}</th>
          <td mat-cell *matCellDef="let row">{{ row.employeeName }}</td>
          <td mat-footer-cell *matFooterCellDef>
            {{ 'common.total' | translate }} ({{ report()?.totals?.employeesCount ?? 0 }})
          </td>
        </ng-container>
        <ng-container matColumnDef="departmentName">
          <th mat-header-cell *matHeaderCellDef>{{ 'employees.department' | translate }}</th>
          <td mat-cell *matCellDef="let row">{{ row.departmentName }}</td>
          <td mat-footer-cell *matFooterCellDef></td>
        </ng-container>
        <ng-container matColumnDef="baseSalary">
          <th mat-header-cell *matHeaderCellDef class="num">{{ 'payroll.baseSalary' | translate }}</th>
          <td mat-cell *matCellDef="let row" class="num">{{ row.baseSalary | money }}</td>
          <td mat-footer-cell *matFooterCellDef class="num">{{ report()?.totals?.baseSalary | money }}</td>
        </ng-container>
        <ng-container matColumnDef="departmentIncentive">
          <th mat-header-cell *matHeaderCellDef class="num">{{ 'payroll.departmentIncentive' | translate }}</th>
          <td mat-cell *matCellDef="let row" class="num">{{ row.departmentIncentive | money }}</td>
          <td mat-footer-cell *matFooterCellDef class="num">{{ report()?.totals?.departmentIncentive | money }}</td>
        </ng-container>
        <ng-container matColumnDef="serviceYearsIncentive">
          <th mat-header-cell *matHeaderCellDef class="num">{{ 'payroll.serviceYearsIncentive' | translate }}</th>
          <td mat-cell *matCellDef="let row" class="num">{{ row.serviceYearsIncentive | money }}</td>
          <td mat-footer-cell *matFooterCellDef class="num">{{ report()?.totals?.serviceYearsIncentive | money }}</td>
        </ng-container>
        <ng-container matColumnDef="absenceBonus">
          <th mat-header-cell *matHeaderCellDef class="num">{{ 'payroll.absenceBonus' | translate }}</th>
          <td mat-cell *matCellDef="let row" class="num positive">{{ row.absenceBonus | money }}</td>
          <td mat-footer-cell *matFooterCellDef class="num positive">{{ report()?.totals?.absenceBonus | money }}</td>
        </ng-container>
        <ng-container matColumnDef="absenceDeduction">
          <th mat-header-cell *matHeaderCellDef class="num">{{ 'payroll.absenceDeduction' | translate }}</th>
          <td mat-cell *matCellDef="let row" class="num negative">{{ row.absenceDeduction | money }}</td>
          <td mat-footer-cell *matFooterCellDef class="num negative">{{ report()?.totals?.absenceDeduction | money }}</td>
        </ng-container>
        <ng-container matColumnDef="netSalary">
          <th mat-header-cell *matHeaderCellDef class="num">{{ 'payroll.netSalary' | translate }}</th>
          <td mat-cell *matCellDef="let row" class="num"><strong>{{ row.netSalary | money }}</strong></td>
          <td mat-footer-cell *matFooterCellDef class="num">{{ report()?.totals?.netSalary | money }}</td>
        </ng-container>
        <tr mat-header-row *matHeaderRowDef="columns"></tr>
        <tr mat-row *matRowDef="let row; columns: columns"></tr>
        <tr mat-footer-row *matFooterRowDef="columns"></tr>
      </table>
      @if (!resource.isLoading() && !report()?.rows?.length) {
        <app-empty-state icon="summarize" [message]="'common.noData' | translate" />
      }
    </div>
  `,
})
export class SalariesReport {
  private readonly api = inject(ReportsApi);
  private readonly language = inject(LanguageService);

  protected readonly filter = signal<MonthlyReportFilter>(currentMonthFilter());
  protected readonly exporting = signal(false);
  protected readonly export = injectReportExport('salaries', this.exporting);
  protected readonly columns = [
    'employeeName',
    'departmentName',
    'baseSalary',
    'departmentIncentive',
    'serviceYearsIncentive',
    'absenceBonus',
    'absenceDeduction',
    'netSalary',
  ];

  protected readonly resource = rxResource({
    params: () => ({ ...this.filter(), lang: this.language.current() }),
    stream: ({ params: { lang: _, ...filter } }) => this.api.salaries(filter),
  });
  protected readonly report = computed(() => (this.resource.hasValue() ? this.resource.value() : null));
}
