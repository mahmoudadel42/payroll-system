import { Component, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslatePipe } from '@ngx-translate/core';
import { debounceTime, distinctUntilChanged, filter, switchMap } from 'rxjs';
import { LanguageService } from '@core/i18n/language.service';
import { NotificationService } from '@core/notifications/notification.service';
import { pagedResource } from '@shared/data-access/paged-resource';
import { HasRoleDirective } from '@shared/directives/has-role.directive';
import { DEFAULT_PAGE_SIZE, PAGE_SIZE_OPTIONS, PagedQuery } from '@shared/models/api.models';
import { PercentValuePipe } from '@shared/pipes/format.pipes';
import { ConfirmService } from '@shared/ui/confirm-dialog/confirm.service';
import { PageHeader } from '@shared/ui/page-header/page-header';
import { EmptyState } from '@shared/ui/state/empty-state';
import { DepartmentFormDialog } from '../../components/department-form-dialog/department-form-dialog';
import { Department } from '../../data-access/department.models';
import { DepartmentsApi } from '../../data-access/departments.api';

@Component({
  selector: 'app-department-list',
  imports: [
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    TranslatePipe,
    PageHeader,
    EmptyState,
    HasRoleDirective,
    PercentValuePipe,
  ],
  templateUrl: './department-list.html',
})
export class DepartmentList {
  private readonly api = inject(DepartmentsApi);
  private readonly dialog = inject(MatDialog);
  private readonly confirm = inject(ConfirmService);
  private readonly notifications = inject(NotificationService);
  protected readonly language = inject(LanguageService);

  protected readonly pageSizeOptions = PAGE_SIZE_OPTIONS;
  protected readonly columns = ['nameEn', 'nameAr', 'incentivePercentage', 'employeesCount', 'actions'];
  protected readonly search = new FormControl('', { nonNullable: true });

  protected readonly list = pagedResource<Department, PagedQuery>({
    initialQuery: { page: 1, pageSize: DEFAULT_PAGE_SIZE, search: '' },
    loader: (query) => this.api.list(query),
  });

  constructor() {
    this.search.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe((search) => this.list.filter({ search }));
  }

  protected openForm(department: Department | null = null): void {
    this.dialog
      .open(DepartmentFormDialog, { data: department, width: '520px' })
      .afterClosed()
      .pipe(filter(Boolean))
      .subscribe(() => this.list.reload());
  }

  protected delete(department: Department): void {
    const name = this.language.current() === 'ar' ? department.nameAr : department.nameEn;
    this.confirm
      .confirmDelete(name)
      .pipe(switchMap(() => this.api.delete(department.id)))
      .subscribe(() => {
        this.notifications.success('common.deletedSuccessfully');
        this.list.reload();
      });
  }
}
