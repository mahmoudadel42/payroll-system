import { Component, computed, input } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

export interface BarItem {
  /** Display text, or a translation key (falls back to the raw text when no key matches). */
  name: string;
  count: number;
}

/** Lightweight horizontal bar chart; avoids pulling a charting library for simple distributions. */
@Component({
  selector: 'app-bar-list',
  imports: [TranslatePipe],
  template: `
    <ul class="bars">
      @for (item of items(); track item.name) {
        <li>
          <div class="bars__label">
            <span>{{ item.name | translate }}</span>
            <strong>{{ item.count }}</strong>
          </div>
          <div class="bars__track">
            <div class="bars__fill" [style.width.%]="(item.count / max()) * 100"></div>
          </div>
        </li>
      }
    </ul>
  `,
  styles: `
    .bars {
      display: flex;
      flex-direction: column;
      gap: 12px;
      margin: 0;
      padding: 0;
      list-style: none;
    }
    .bars__label {
      display: flex;
      justify-content: space-between;
      margin-block-end: 4px;
      font: var(--mat-sys-body-medium);
    }
    .bars__track {
      height: 8px;
      border-radius: 4px;
      background: var(--mat-sys-surface-container-high);
      overflow: hidden;
    }
    .bars__fill {
      height: 100%;
      border-radius: 4px;
      background: var(--mat-sys-primary);
      transition: width 300ms ease;
    }
  `,
})
export class BarList {
  readonly items = input.required<BarItem[]>();
  protected readonly max = computed(() => Math.max(1, ...this.items().map((i) => i.count)));
}
