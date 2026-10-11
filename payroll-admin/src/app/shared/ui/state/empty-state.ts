import { Component, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-empty-state',
  imports: [MatIconModule],
  template: `
    <div class="empty-state">
      <mat-icon>{{ icon() }}</mat-icon>
      <p>{{ message() }}</p>
    </div>
  `,
  styles: `
    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 8px;
      padding: 48px 16px;
      color: var(--mat-sys-on-surface-variant);
      text-align: center;
    }
    mat-icon {
      width: 48px;
      height: 48px;
      font-size: 48px;
      opacity: 0.6;
    }
    p {
      margin: 0;
    }
  `,
})
export class EmptyState {
  readonly message = input.required<string>();
  readonly icon = input('inbox');
}
