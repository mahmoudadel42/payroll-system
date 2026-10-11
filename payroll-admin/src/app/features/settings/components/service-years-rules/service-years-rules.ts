import { Component, computed, inject } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { filter, switchMap } from 'rxjs';
import { NotificationService } from '@core/notifications/notification.service';
import { HasRoleDirective } from '@shared/directives/has-role.directive';
import { PercentValuePipe } from '@shared/pipes/format.pipes';
import { ConfirmService } from '@shared/ui/confirm-dialog/confirm.service';
import { EmptyState } from '@shared/ui/state/empty-state';
import { ServiceYearsRule } from '../../data-access/settings.models';
import { SettingsApi } from '../../data-access/settings.api';
import { ServiceYearsRuleDialog } from './service-years-rule-dialog';

@Component({
  selector: 'app-service-years-rules',
  imports: [
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatProgressBarModule,
    TranslatePipe,
    HasRoleDirective,
    EmptyState,
    PercentValuePipe,
  ],
  template: `
    <div class="tab-toolbar">
      <p class="muted">{{ 'settings.serviceYearsHint' | translate }}</p>
      <button mat-flat-button *appHasRole="'Admin'" (click)="openForm()">
        <mat-icon>add</mat-icon>
        {{ 'settings.addRule' | translate }}
      </button>
    </div>
    <div class="table-wrapper">
      @if (resource.isLoading()) {
        <mat-progress-bar class="loading-bar" mode="indeterminate" />
      }
      <table mat-table [dataSource]="rows()">
        <ng-container matColumnDef="minYears">
          <th mat-header-cell *matHeaderCellDef>{{ 'settings.minYears' | translate }}</th>
          <td mat-cell *matCellDef="let row">{{ 'settings.yearsOrMore' | translate: { years: row.minYears } }}</td>
        </ng-container>
        <ng-container matColumnDef="percentage">
          <th mat-header-cell *matHeaderCellDef class="num">{{ 'settings.percentage' | translate }}</th>
          <td mat-cell *matCellDef="let row" class="num">{{ row.percentage | percentValue }}</td>
        </ng-container>
        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef class="actions"></th>
          <td mat-cell *matCellDef="let row" class="actions">
            <ng-container *appHasRole="'Admin'">
              <button mat-icon-button (click)="openForm(row)" [matTooltip]="'common.edit' | translate">
                <mat-icon>edit</mat-icon>
              </button>
              <button mat-icon-button (click)="delete(row)" [matTooltip]="'common.delete' | translate">
                <mat-icon>delete</mat-icon>
              </button>
            </ng-container>
          </td>
        </ng-container>
        <tr mat-header-row *matHeaderRowDef="columns"></tr>
        <tr mat-row *matRowDef="let row; columns: columns"></tr>
      </table>
      @if (!resource.isLoading() && rows().length === 0) {
        <app-empty-state icon="workspace_premium" [message]="'common.noData' | translate" />
      }
    </div>
  `,
})
export class ServiceYearsRules {
  private readonly api = inject(SettingsApi);
  private readonly dialog = inject(MatDialog);
  private readonly confirm = inject(ConfirmService);
  private readonly notifications = inject(NotificationService);
  private readonly translate = inject(TranslateService);

  protected readonly resource = rxResource({ stream: () => this.api.serviceYearsRules() });
  protected readonly rows = computed(() => (this.resource.hasValue() ? this.resource.value() : []));
  protected readonly columns = ['minYears', 'percentage', 'actions'];

  protected openForm(rule: ServiceYearsRule | null = null): void {
    this.dialog
      .open(ServiceYearsRuleDialog, { data: rule, width: '480px' })
      .afterClosed()
      .pipe(filter(Boolean))
      .subscribe(() => this.resource.reload());
  }

  protected delete(rule: ServiceYearsRule): void {
    const name = this.translate.instant('settings.yearsOrMore', { years: rule.minYears });
    this.confirm
      .confirmDelete(name)
      .pipe(switchMap(() => this.api.deleteServiceYearsRule(rule.id)))
      .subscribe(() => {
        this.notifications.success('common.deletedSuccessfully');
        this.resource.reload();
      });
  }
}
