import { Component, inject } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from '@core/auth/auth.service';
import { PageHeader } from '@shared/ui/page-header/page-header';
import { AbsenceRules } from '../../components/absence-rules/absence-rules';
import { GradeSalaries } from '../../components/grade-salaries/grade-salaries';
import { ServiceYearsRules } from '../../components/service-years-rules/service-years-rules';

@Component({
  selector: 'app-settings-page',
  imports: [MatTabsModule, MatIconModule, TranslatePipe, PageHeader, GradeSalaries, ServiceYearsRules, AbsenceRules],
  template: `
    <app-page-header [title]="'nav.settings' | translate" [subtitle]="'settings.subtitle' | translate" />

    @if (!auth.isAdmin()) {
      <p class="read-only muted">
        <mat-icon>visibility</mat-icon>
        {{ 'settings.readOnly' | translate }}
      </p>
    }

    <section class="card">
      <mat-tab-group mat-stretch-tabs="false" animationDuration="150ms">
        <mat-tab [label]="'settings.gradeSalaries' | translate">
          <app-grade-salaries />
        </mat-tab>
        <mat-tab [label]="'settings.serviceYearsRules' | translate">
          <app-service-years-rules />
        </mat-tab>
        <mat-tab [label]="'settings.absenceRules' | translate">
          <app-absence-rules />
        </mat-tab>
      </mat-tab-group>
    </section>
  `,
  styles: `
    .read-only {
      display: flex;
      align-items: center;
      gap: 8px;
      margin: 0 0 16px;
    }
  `,
})
export class SettingsPage {
  protected readonly auth = inject(AuthService);
}
