import { Component, computed, inject, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { TranslatePipe } from '@ngx-translate/core';
import { LanguageService } from '@core/i18n/language.service';
import { injectDepartmentOptions } from '@shared/data-access/lookups.api';
import { JOB_GRADES } from '@shared/models/api.models';
import { LocalDatePipe, MoneyPipe } from '@shared/pipes/format.pipes';
import { BarList } from '@shared/ui/bar-list/bar-list';
import { EmptyState } from '@shared/ui/state/empty-state';
import { injectReportExport } from '../data-access/report-export';
import { EmployeesReportFilter } from '../data-access/report.models';
import { ReportsApi } from '../data-access/reports.api';

@Component({
  selector: 'app-employees-report',
  imports: [
    MatTableModule,
    MatProgressBarModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    TranslatePipe,
    MoneyPipe,
    LocalDatePipe,
    EmptyState,
    BarList,
  ],
  template: `
    <div class="filters">
      <mat-form-field>
        <mat-label>{{ 'employees.department' | translate }}</mat-label>
        <mat-select [value]="filter().departmentId" (selectionChange)="patch({ departmentId: $event.value })">
          <mat-option [value]="null">{{ 'common.all' | translate }}</mat-option>
          @for (department of departments(); track department.id) {
            <mat-option [value]="department.id">{{ department.name }}</mat-option>
          }
        </mat-select>
      </mat-form-field>
      <mat-form-field>
        <mat-label>{{ 'employees.grade' | translate }}</mat-label>
        <mat-select [value]="filter().grade" (selectionChange)="patch({ grade: $event.value })">
          <mat-option [value]="null">{{ 'common.all' | translate }}</mat-option>
          @for (grade of grades; track grade) {
            <mat-option [value]="grade">{{ 'enums.grade.' + grade | translate }}</mat-option>
          }
        </mat-select>
      </mat-form-field>
      <span class="spacer"></span>
      <button mat-stroked-button (click)="export(filter())" [disabled]="exporting()">
        <mat-icon>download</mat-icon>
        {{ 'reports.exportExcel' | translate }}
      </button>
    </div>

    @if (report(); as report) {
      <div class="breakdown">
        <div>
          <h3>{{ 'reports.byDepartment' | translate }}</h3>
          <app-bar-list [items]="report.byDepartment" />
        </div>
        <div>
          <h3>{{ 'reports.byGrade' | translate }}</h3>
          <app-bar-list [items]="gradeItems()" />
        </div>
      </div>
    }

    <div class="table-wrapper">
      @if (resource.isLoading()) {
        <mat-progress-bar class="loading-bar" mode="indeterminate" />
      }
      <table mat-table [dataSource]="report()?.rows ?? []">
        <ng-container matColumnDef="fullName">
          <th mat-header-cell *matHeaderCellDef>{{ 'employees.fullName' | translate }}</th>
          <td mat-cell *matCellDef="let row">
            <div>{{ row.fullName }}</div>
            <small class="muted" dir="ltr">{{ row.email }}</small>
          </td>
        </ng-container>
        <ng-container matColumnDef="departmentName">
          <th mat-header-cell *matHeaderCellDef>{{ 'employees.department' | translate }}</th>
          <td mat-cell *matCellDef="let row">{{ row.departmentName }}</td>
        </ng-container>
        <ng-container matColumnDef="grade">
          <th mat-header-cell *matHeaderCellDef>{{ 'employees.grade' | translate }}</th>
          <td mat-cell *matCellDef="let row">{{ 'enums.grade.' + row.grade | translate }}</td>
        </ng-container>
        <ng-container matColumnDef="hireDate">
          <th mat-header-cell *matHeaderCellDef>{{ 'employees.hireDate' | translate }}</th>
          <td mat-cell *matCellDef="let row">{{ row.hireDate | localDate }}</td>
        </ng-container>
        <ng-container matColumnDef="serviceYears">
          <th mat-header-cell *matHeaderCellDef class="num">{{ 'employees.serviceYears' | translate }}</th>
          <td mat-cell *matCellDef="let row" class="num">{{ row.serviceYears }}</td>
        </ng-container>
        <ng-container matColumnDef="baseSalary">
          <th mat-header-cell *matHeaderCellDef class="num">{{ 'payroll.baseSalary' | translate }}</th>
          <td mat-cell *matCellDef="let row" class="num">{{ row.baseSalary | money }}</td>
        </ng-container>
        <tr mat-header-row *matHeaderRowDef="columns"></tr>
        <tr mat-row *matRowDef="let row; columns: columns"></tr>
      </table>
      @if (!resource.isLoading() && !report()?.rows?.length) {
        <app-empty-state icon="groups" [message]="'common.noData' | translate" />
      }
    </div>
  `,
  styles: `
    .spacer {
      flex: 1;
    }
    .breakdown {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
      gap: 24px;
      padding: 20px;
      border-block-end: 1px solid var(--mat-sys-outline-variant);
    }
    h3 {
      margin: 0 0 12px;
      font: var(--mat-sys-title-small);
    }
  `,
})
export class EmployeesReport {
  private readonly api = inject(ReportsApi);
  private readonly language = inject(LanguageService);

  protected readonly grades = JOB_GRADES;
  protected readonly departments = injectDepartmentOptions();
  protected readonly filter = signal<EmployeesReportFilter>({ departmentId: null, grade: null });
  protected readonly exporting = signal(false);
  protected readonly export = injectReportExport('employees', this.exporting);
  protected readonly columns = ['fullName', 'departmentName', 'grade', 'hireDate', 'serviceYears', 'baseSalary'];

  protected readonly resource = rxResource({
    params: () => ({ ...this.filter(), lang: this.language.current() }),
    stream: ({ params: { lang: _, ...filter } }) => this.api.employees(filter),
  });
  protected readonly report = computed(() => (this.resource.hasValue() ? this.resource.value() : null));

  /** The API returns grade enum names; the bar labels need translation keys. */
  protected readonly gradeItems = computed(() =>
    (this.report()?.byGrade ?? []).map((item) => ({ ...item, name: `enums.grade.${item.name}` })),
  );

  protected patch(changes: Partial<EmployeesReportFilter>): void {
    this.filter.update((current) => ({ ...current, ...changes }));
  }
}
