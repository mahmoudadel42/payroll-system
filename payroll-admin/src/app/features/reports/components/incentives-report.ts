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
  selector: 'app-incentives-report',
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
          <td mat-cell *matCellDef="let row">
            <div>{{ row.employeeName }}</div>
            <small class="muted">{{ row.departmentName }}</small>
          </td>
          <td mat-footer-cell *matFooterCellDef>{{ 'common.total' | translate }}</td>
        </ng-container>
        <ng-container matColumnDef="serviceYears">
          <th mat-header-cell *matHeaderCellDef class="num">{{ 'employees.serviceYears' | translate }}</th>
          <td mat-cell *matCellDef="let row" class="num">{{ row.serviceYears }}</td>
          <td mat-footer-cell *matFooterCellDef></td>
        </ng-container>
        <ng-container matColumnDef="absenceDays">
          <th mat-header-cell *matHeaderCellDef class="num">{{ 'payroll.absenceDays' | translate }}</th>
          <td mat-cell *matCellDef="let row" class="num">{{ row.absenceDays }}</td>
          <td mat-footer-cell *matFooterCellDef></td>
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
        <ng-container matColumnDef="netEffect">
          <th mat-header-cell *matHeaderCellDef class="num">{{ 'reports.netEffect' | translate }}</th>
          <td mat-cell *matCellDef="let row" class="num" [class.negative]="row.netEffect < 0">
            <strong>{{ row.netEffect | money }}</strong>
          </td>
          <td mat-footer-cell *matFooterCellDef class="num">{{ report()?.totals?.netEffect | money }}</td>
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
export class IncentivesReport {
  private readonly api = inject(ReportsApi);
  private readonly language = inject(LanguageService);

  protected readonly filter = signal<MonthlyReportFilter>(currentMonthFilter());
  protected readonly exporting = signal(false);
  protected readonly export = injectReportExport('incentives-deductions', this.exporting);
  protected readonly columns = [
    'employeeName',
    'serviceYears',
    'absenceDays',
    'departmentIncentive',
    'serviceYearsIncentive',
    'absenceBonus',
    'absenceDeduction',
    'netEffect',
  ];

  protected readonly resource = rxResource({
    params: () => ({ ...this.filter(), lang: this.language.current() }),
    stream: ({ params: { lang: _, ...filter } }) => this.api.incentivesDeductions(filter),
  });
  protected readonly report = computed(() => (this.resource.hasValue() ? this.resource.value() : null));
}
