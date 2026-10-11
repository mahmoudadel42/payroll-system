import { Component, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslatePipe } from '@ngx-translate/core';
import { debounceTime, distinctUntilChanged, filter, switchMap } from 'rxjs';
import { NotificationService } from '@core/notifications/notification.service';
import { injectDepartmentOptions } from '@shared/data-access/lookups.api';
import { pagedResource } from '@shared/data-access/paged-resource';
import { DEFAULT_PAGE_SIZE, PAGE_SIZE_OPTIONS } from '@shared/models/api.models';
import { LocalDatePipe } from '@shared/pipes/format.pipes';
import { ConfirmService } from '@shared/ui/confirm-dialog/confirm.service';
import { PageHeader } from '@shared/ui/page-header/page-header';
import { EmptyState } from '@shared/ui/state/empty-state';
import { toIsoDate } from '@shared/utils/date';
import { AbsenceCreateDialog } from '../../components/absence-create-dialog/absence-create-dialog';
import { AbsenceEditDialog } from '../../components/absence-edit-dialog/absence-edit-dialog';
import { Absence, AbsencesQuery } from '../../data-access/absence.models';
import { AbsencesApi } from '../../data-access/absences.api';

@Component({
  selector: 'app-absence-list',
  imports: [
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    TranslatePipe,
    PageHeader,
    EmptyState,
    LocalDatePipe,
  ],
  templateUrl: './absence-list.html',
})
export class AbsenceList {
  private readonly api = inject(AbsencesApi);
  private readonly dialog = inject(MatDialog);
  private readonly confirm = inject(ConfirmService);
  private readonly notifications = inject(NotificationService);

  protected readonly departments = injectDepartmentOptions();
  protected readonly pageSizeOptions = PAGE_SIZE_OPTIONS;
  protected readonly columns = ['employeeName', 'departmentName', 'date', 'reason', 'actions'];
  protected readonly search = new FormControl('', { nonNullable: true });
  protected readonly from = new FormControl<Date | null>(null);
  protected readonly to = new FormControl<Date | null>(null);

  protected readonly list = pagedResource<Absence, AbsencesQuery>({
    initialQuery: { page: 1, pageSize: DEFAULT_PAGE_SIZE, search: '', departmentId: null, from: null, to: null },
    loader: (query) => this.api.list(query),
    localized: true,
  });

  constructor() {
    this.search.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe((search) => this.list.filter({ search }));
  }

  protected filterByDepartment(departmentId: number | null): void {
    this.list.filter({ departmentId });
  }

  /** Applied when the range picker closes so a half-picked range doesn't trigger a request. */
  protected applyDateRange(): void {
    this.list.filter({ from: toIsoDate(this.from.value), to: toIsoDate(this.to.value) });
  }

  protected clearDateRange(): void {
    this.from.setValue(null);
    this.to.setValue(null);
    this.applyDateRange();
  }

  protected create(): void {
    this.dialog
      .open(AbsenceCreateDialog, { width: '560px' })
      .afterClosed()
      .pipe(filter(Boolean))
      .subscribe(() => this.list.reload());
  }

  protected edit(absence: Absence): void {
    this.dialog
      .open(AbsenceEditDialog, { data: absence, width: '480px' })
      .afterClosed()
      .pipe(filter(Boolean))
      .subscribe(() => this.list.reload());
  }

  protected delete(absence: Absence): void {
    this.confirm
      .confirmDelete(absence.employeeName)
      .pipe(switchMap(() => this.api.delete(absence.id)))
      .subscribe(() => {
        this.notifications.success('common.deletedSuccessfully');
        this.list.reload();
      });
  }
}
