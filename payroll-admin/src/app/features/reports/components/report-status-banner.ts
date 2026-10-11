import { Component, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { TranslatePipe } from '@ngx-translate/core';
import { PayrollRunStatus } from '@shared/models/api.models';

/** Tells the user whether figures come from a saved payroll run or a live preview calculation. */
@Component({
  selector: 'app-report-status-banner',
  imports: [MatIconModule, TranslatePipe],
  template: `
    <div class="banner" [class.banner--preview]="isPreview()">
      <mat-icon>{{ isPreview() ? 'preview' : 'task_alt' }}</mat-icon>
      @if (isPreview()) {
        {{ 'reports.previewHint' | translate }}
      } @else {
        {{ 'reports.fromRun' | translate: { status: ('enums.runStatus.' + runStatus() | translate) } }}
      }
    </div>
  `,
  styles: `
    .banner {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 10px 20px;
      background: var(--app-success-container);
      color: var(--app-success);
      font: var(--mat-sys-body-medium);
    }
    .banner--preview {
      background: var(--app-warning-container);
      color: var(--app-warning);
    }
  `,
})
export class ReportStatusBanner {
  readonly isPreview = input.required<boolean>();
  readonly runStatus = input<PayrollRunStatus | null>(null);
}
