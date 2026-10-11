import { Component, computed, inject } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from '@core/auth/auth.service';
import { LanguageService } from '@core/i18n/language.service';
import { RunStatusChip } from '@features/payroll-runs/components/run-status-chip';
import { PayrollRunsApi } from '@features/payroll-runs/data-access/payroll-runs.api';
import { ReportsApi } from '@features/reports/data-access/reports.api';
import { MoneyPipe, PeriodPipe } from '@shared/pipes/format.pipes';
import { BarList } from '@shared/ui/bar-list/bar-list';
import { EmptyState } from '@shared/ui/state/empty-state';

@Component({
  selector: 'app-dashboard',
  imports: [
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    TranslatePipe,
    MoneyPipe,
    PeriodPipe,
    RunStatusChip,
    BarList,
    EmptyState,
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  private readonly reportsApi = inject(ReportsApi);
  private readonly payrollApi = inject(PayrollRunsApi);
  private readonly language = inject(LanguageService);
  protected readonly auth = inject(AuthService);

  private readonly workforce = rxResource({
    params: () => this.language.current(),
    stream: () => this.reportsApi.employees({}),
  });

  private readonly runs = rxResource({
    stream: () => this.payrollApi.list({ page: 1, pageSize: 5 }),
  });

  protected readonly isLoading = computed(() => this.workforce.isLoading() || this.runs.isLoading());
  protected readonly report = computed(() => (this.workforce.hasValue() ? this.workforce.value() : null));
  protected readonly recentRuns = computed(() => (this.runs.hasValue() ? this.runs.value().items : []));
  protected readonly latestRun = computed(() => this.recentRuns()[0] ?? null);
  protected readonly draftCount = computed(() => this.recentRuns().filter((r) => r.status === 'Draft').length);

  protected readonly gradeItems = computed(() =>
    (this.report()?.byGrade ?? []).map((item) => ({ ...item, name: `enums.grade.${item.name}` })),
  );

  protected readonly greetingKey = computed(() => {
    const hour = new Date().getHours();
    return hour < 12 ? 'dashboard.goodMorning' : hour < 18 ? 'dashboard.goodAfternoon' : 'dashboard.goodEvening';
  });
}
