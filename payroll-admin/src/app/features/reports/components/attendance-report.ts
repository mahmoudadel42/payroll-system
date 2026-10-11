import { Component, computed, inject, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { TranslatePipe } from '@ngx-translate/core';
import { LanguageService } from '@core/i18n/language.service';
import { PercentValuePipe } from '@shared/pipes/format.pipes';
import { EmptyState } from '@shared/ui/state/empty-state';
import { injectReportExport } from '../data-access/report-export';
import { MonthlyReportFilter, currentMonthFilter } from '../data-access/report.models';
import { ReportsApi } from '../data-access/reports.api';
import { MonthlyFilterBar } from './monthly-filter-bar';

@Component({
  selector: 'app-attendance-report',
  imports: [MatTableModule, MatProgressBarModule, TranslatePipe, PercentValuePipe, EmptyState, MonthlyFilterBar],
  template: `
    <app-monthly-filter-bar [(filter)]="filter" [exporting]="exporting()" (export)="export(filter())" />
    @if (report(); as report) {
      <div class="kpis">
        <div><span class="muted">{{ 'reports.totalAbsenceDays' | translate }}</span><strong>{{ report.totalAbsenceDays }}</strong></div>
        <div><span class="muted">{{ 'reports.employeesWithBonus' | translate }}</span><strong class="positive">{{ report.employeesWithBonus }}</strong></div>
        <div><span class="muted">{{ 'reports.employeesWithDeduction' | translate }}</span><strong class="negative">{{ report.employeesWithDeduction }}</strong></div>
      </div>
    }
    <div class="table-wrapper">
      @if (resource.isLoading()) {
        <mat-progress-bar class="loading-bar" mode="indeterminate" />
      }
      <table mat-table [dataSource]="report()?.rows ?? []">
        <ng-container matColumnDef="employeeName">
          <th mat-header-cell *matHeaderCellDef>{{ 'absences.employee' | translate }}</th>
          <td mat-cell *matCellDef="let row">{{ row.employeeName }}</td>
        </ng-container>
        <ng-container matColumnDef="departmentName">
          <th mat-header-cell *matHeaderCellDef>{{ 'employees.department' | translate }}</th>
          <td mat-cell *matCellDef="let row">{{ row.departmentName }}</td>
        </ng-container>
        <ng-container matColumnDef="absenceDays">
          <th mat-header-cell *matHeaderCellDef class="num">{{ 'payroll.absenceDays' | translate }}</th>
          <td mat-cell *matCellDef="let row" class="num">{{ row.absenceDays }}</td>
        </ng-container>
        <ng-container matColumnDef="dates">
          <th mat-header-cell *matHeaderCellDef>{{ 'reports.dates' | translate }}</th>
          <td mat-cell *matCellDef="let row" class="muted" dir="ltr">{{ row.dates || '—' }}</td>
        </ng-container>
        <ng-container matColumnDef="effect">
          <th mat-header-cell *matHeaderCellDef>{{ 'reports.effect' | translate }}</th>
          <td mat-cell *matCellDef="let row">
            <span class="chip" [class.chip--success]="row.effect === 'Bonus'" [class.chip--error]="row.effect === 'Deduction'">
              {{ 'enums.absenceEffect.' + row.effect | translate }}
              @if (row.effect !== 'None') {
                · {{ row.effectPercentage | percentValue }}
              }
            </span>
          </td>
        </ng-container>
        <tr mat-header-row *matHeaderRowDef="columns"></tr>
        <tr mat-row *matRowDef="let row; columns: columns"></tr>
      </table>
      @if (!resource.isLoading() && !report()?.rows?.length) {
        <app-empty-state icon="event_available" [message]="'common.noData' | translate" />
      }
    </div>
  `,
  styles: `
    .kpis {
      display: flex;
      flex-wrap: wrap;
      gap: 32px;
      padding: 16px 20px;
      border-block-end: 1px solid var(--mat-sys-outline-variant);
    }
    .kpis div {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }
    .kpis strong {
      font: var(--mat-sys-title-large);
    }
  `,
})
export class AttendanceReport {
  private readonly api = inject(ReportsApi);
  private readonly language = inject(LanguageService);

  protected readonly filter = signal<MonthlyReportFilter>(currentMonthFilter());
  protected readonly exporting = signal(false);
  protected readonly export = injectReportExport('attendance', this.exporting);
  protected readonly columns = ['employeeName', 'departmentName', 'absenceDays', 'dates', 'effect'];

  protected readonly resource = rxResource({
    params: () => ({ ...this.filter(), lang: this.language.current() }),
    stream: ({ params: { lang: _, ...filter } }) => this.api.attendance(filter),
  });
  protected readonly report = computed(() => (this.resource.hasValue() ? this.resource.value() : null));
}
