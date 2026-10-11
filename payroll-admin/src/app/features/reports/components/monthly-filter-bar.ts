import { Component, computed, inject, input, model, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { TranslatePipe } from '@ngx-translate/core';
import { LanguageService } from '@core/i18n/language.service';
import { injectDepartmentOptions } from '@shared/data-access/lookups.api';
import { monthNames, recentYears } from '@shared/utils/date';
import { MonthlyReportFilter } from '../data-access/report.models';

@Component({
  selector: 'app-monthly-filter-bar',
  imports: [MatFormFieldModule, MatSelectModule, MatButtonModule, MatIconModule, TranslatePipe],
  template: `
    <div class="filters">
      <mat-form-field>
        <mat-label>{{ 'common.month' | translate }}</mat-label>
        <mat-select [value]="filter().month" (selectionChange)="patch({ month: $event.value })">
          @for (name of months(); track $index) {
            <mat-option [value]="$index + 1">{{ name }}</mat-option>
          }
        </mat-select>
      </mat-form-field>
      <mat-form-field>
        <mat-label>{{ 'common.year' | translate }}</mat-label>
        <mat-select [value]="filter().year" (selectionChange)="patch({ year: $event.value })">
          @for (year of years; track year) {
            <mat-option [value]="year">{{ year }}</mat-option>
          }
        </mat-select>
      </mat-form-field>
      <mat-form-field>
        <mat-label>{{ 'employees.department' | translate }}</mat-label>
        <mat-select [value]="filter().departmentId" (selectionChange)="patch({ departmentId: $event.value })">
          <mat-option [value]="null">{{ 'common.all' | translate }}</mat-option>
          @for (department of departments(); track department.id) {
            <mat-option [value]="department.id">{{ department.name }}</mat-option>
          }
        </mat-select>
      </mat-form-field>
      <span class="spacer"></span>
      <button mat-stroked-button (click)="export.emit()" [disabled]="exporting()">
        <mat-icon>download</mat-icon>
        {{ 'reports.exportExcel' | translate }}
      </button>
    </div>
  `,
  styles: `
    .spacer {
      flex: 1;
    }
  `,
})
export class MonthlyFilterBar {
  private readonly language = inject(LanguageService);

  readonly filter = model.required<MonthlyReportFilter>();
  readonly exporting = input(false);
  readonly export = output();

  protected readonly years = recentYears();
  protected readonly months = computed(() => monthNames(this.language.locale()));
  protected readonly departments = injectDepartmentOptions();

  protected patch(changes: Partial<MonthlyReportFilter>): void {
    this.filter.update((current) => ({ ...current, ...changes }));
  }
}
