import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { TranslatePipe } from '@ngx-translate/core';
import { PageHeader } from '@shared/ui/page-header/page-header';
import { AttendanceReport } from '../../components/attendance-report';
import { EmployeesReport } from '../../components/employees-report';
import { IncentivesReport } from '../../components/incentives-report';
import { SalariesReport } from '../../components/salaries-report';

@Component({
  selector: 'app-reports-page',
  imports: [MatTabsModule, TranslatePipe, PageHeader, SalariesReport, IncentivesReport, AttendanceReport, EmployeesReport],
  template: `
    <app-page-header [title]="'nav.reports' | translate" [subtitle]="'reports.subtitle' | translate" />
    <section class="card">
      <mat-tab-group mat-stretch-tabs="false" animationDuration="150ms">
        <mat-tab [label]="'reports.salaries' | translate">
          <ng-template matTabContent><app-salaries-report /></ng-template>
        </mat-tab>
        <mat-tab [label]="'reports.incentivesDeductions' | translate">
          <ng-template matTabContent><app-incentives-report /></ng-template>
        </mat-tab>
        <mat-tab [label]="'reports.attendance' | translate">
          <ng-template matTabContent><app-attendance-report /></ng-template>
        </mat-tab>
        <mat-tab [label]="'reports.employees' | translate">
          <ng-template matTabContent><app-employees-report /></ng-template>
        </mat-tab>
      </mat-tab-group>
    </section>
  `,
})
export class ReportsPage {}
