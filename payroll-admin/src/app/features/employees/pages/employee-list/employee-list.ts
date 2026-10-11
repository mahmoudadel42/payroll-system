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
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { debounceTime, distinctUntilChanged, filter, switchMap } from 'rxjs';
import { NotificationService } from '@core/notifications/notification.service';
import { injectDepartmentOptions } from '@shared/data-access/lookups.api';
import { pagedResource } from '@shared/data-access/paged-resource';
import { DEFAULT_PAGE_SIZE, JOB_GRADES, JobGrade, PAGE_SIZE_OPTIONS } from '@shared/models/api.models';
import { LocalDatePipe } from '@shared/pipes/format.pipes';
import { ConfirmService } from '@shared/ui/confirm-dialog/confirm.service';
import { PageHeader } from '@shared/ui/page-header/page-header';
import { EmptyState } from '@shared/ui/state/empty-state';
import { EmployeeFormDialog } from '../../components/employee-form-dialog/employee-form-dialog';
import { Employee, EmployeesQuery } from '../../data-access/employee.models';
import { EmployeesApi } from '../../data-access/employees.api';

@Component({
  selector: 'app-employee-list',
  imports: [
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    TranslatePipe,
    PageHeader,
    EmptyState,
    LocalDatePipe,
  ],
  templateUrl: './employee-list.html',
})
export class EmployeeList {
  private readonly api = inject(EmployeesApi);
  private readonly dialog = inject(MatDialog);
  private readonly router = inject(Router);
  private readonly confirm = inject(ConfirmService);
  private readonly notifications = inject(NotificationService);

  protected readonly grades = JOB_GRADES;
  protected readonly departments = injectDepartmentOptions();
  protected readonly pageSizeOptions = PAGE_SIZE_OPTIONS;
  protected readonly columns = ['fullName', 'department', 'grade', 'contact', 'hireDate', 'actions'];
  protected readonly search = new FormControl('', { nonNullable: true });

  protected readonly list = pagedResource<Employee, EmployeesQuery>({
    initialQuery: { page: 1, pageSize: DEFAULT_PAGE_SIZE, search: '', departmentId: null, grade: null },
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

  protected filterByGrade(grade: JobGrade | null): void {
    this.list.filter({ grade });
  }

  protected openDetails(employee: Employee): void {
    void this.router.navigate(['/employees', employee.id]);
  }

  protected openForm(employee: Employee | null = null, event?: Event): void {
    event?.stopPropagation();
    this.dialog
      .open(EmployeeFormDialog, { data: employee, width: '720px' })
      .afterClosed()
      .pipe(filter(Boolean))
      .subscribe(() => this.list.reload());
  }

  protected delete(employee: Employee, event: Event): void {
    event.stopPropagation();
    this.confirm
      .confirmDelete(employee.fullName)
      .pipe(switchMap(() => this.api.delete(employee.id)))
      .subscribe(() => {
        this.notifications.success('common.deletedSuccessfully');
        this.list.reload();
      });
  }
}
