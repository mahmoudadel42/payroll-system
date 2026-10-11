import { Component, input } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { PayrollRunStatus } from '@shared/models/api.models';

@Component({
  selector: 'app-run-status-chip',
  imports: [TranslatePipe],
  template: `
    <span class="chip" [class.chip--success]="status() === 'Approved'" [class.chip--warning]="status() === 'Draft'">
      {{ 'enums.runStatus.' + status() | translate }}
    </span>
  `,
})
export class RunStatusChip {
  readonly status = input.required<PayrollRunStatus>();
}
