import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { TranslatePipe } from '@ngx-translate/core';
import { MoneyPipe, PercentValuePipe, PeriodPipe } from '@shared/pipes/format.pipes';
import { Payslip } from '../../data-access/payroll-run.models';

export interface PayslipDialogData {
  payslip: Payslip;
  year: number;
  month: number;
}

@Component({
  selector: 'app-payslip-dialog',
  imports: [MatDialogModule, MatButtonModule, MatDividerModule, TranslatePipe, MoneyPipe, PercentValuePipe, PeriodPipe],
  templateUrl: './payslip-dialog.html',
  styleUrl: './payslip-dialog.scss',
})
export class PayslipDialog {
  protected readonly data = inject<PayslipDialogData>(MAT_DIALOG_DATA);
  protected readonly p = this.data.payslip;

  protected print(): void {
    window.print();
  }
}
