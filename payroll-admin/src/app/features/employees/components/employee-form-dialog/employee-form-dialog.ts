import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { TranslatePipe } from '@ngx-translate/core';
import { Observable, finalize } from 'rxjs';
import { NotificationService } from '@core/notifications/notification.service';
import { injectDepartmentOptions } from '@shared/data-access/lookups.api';
import { ControlError } from '@shared/forms/control-error';
import { reportFormError } from '@shared/forms/server-errors';
import { JOB_GRADES, JobGrade } from '@shared/models/api.models';
import { fromIsoDate, toIsoDate, today } from '@shared/utils/date';
import { Employee, SaveEmployeeRequest } from '../../data-access/employee.models';
import { EmployeesApi } from '../../data-access/employees.api';

@Component({
  selector: 'app-employee-form-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatButtonModule,
    TranslatePipe,
    ControlError,
  ],
  templateUrl: './employee-form-dialog.html',
})
export class EmployeeFormDialog {
  private readonly api = inject(EmployeesApi);
  private readonly dialogRef = inject(MatDialogRef<EmployeeFormDialog, boolean>);
  private readonly notifications = inject(NotificationService);
  protected readonly employee = inject<Employee | null>(MAT_DIALOG_DATA);

  protected readonly grades = JOB_GRADES;
  protected readonly departments = injectDepartmentOptions();
  protected readonly maxDate = today();
  protected readonly saving = signal(false);

  private readonly fb = inject(NonNullableFormBuilder);
  protected readonly form = this.fb.group({
    fullName: [this.employee?.fullName ?? '', [Validators.required, Validators.maxLength(200)]],
    email: [this.employee?.email ?? '', [Validators.required, Validators.email, Validators.maxLength(200)]],
    phone: [this.employee?.phone ?? '', [Validators.required, Validators.maxLength(30)]],
    address: [this.employee?.address ?? '', [Validators.required, Validators.maxLength(500)]],
    birthDate: this.fb.control<Date | null>(fromIsoDate(this.employee?.birthDate), Validators.required),
    hireDate: this.fb.control<Date | null>(fromIsoDate(this.employee?.hireDate), Validators.required),
    grade: this.fb.control<JobGrade | null>(this.employee?.grade ?? null, Validators.required),
    departmentId: this.fb.control<number | null>(this.employee?.departmentId ?? null, Validators.required),
  });

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    const request: SaveEmployeeRequest = {
      fullName: value.fullName.trim(),
      email: value.email.trim(),
      phone: value.phone.trim(),
      address: value.address.trim(),
      birthDate: toIsoDate(value.birthDate)!,
      hireDate: toIsoDate(value.hireDate)!,
      grade: value.grade!,
      departmentId: value.departmentId!,
    };

    this.saving.set(true);
    const save$: Observable<unknown> = this.employee ? this.api.update(this.employee.id, request) : this.api.create(request);
    save$.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.notifications.success('common.savedSuccessfully');
        this.dialogRef.close(true);
      },
      error: (error: unknown) => reportFormError(this.form, error, this.notifications),
    });
  }
}
