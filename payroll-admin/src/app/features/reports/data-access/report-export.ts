import { WritableSignal, inject } from '@angular/core';
import { finalize } from 'rxjs';
import { saveBlobResponse } from '@shared/utils/download';
import { EmployeesReportFilter, MonthlyReportFilter, ReportKind } from './report.models';
import { ReportsApi } from './reports.api';

/** Returns an export function bound to a report kind. Call in an injection context. */
export function injectReportExport(kind: ReportKind, busy: WritableSignal<boolean>) {
  const api = inject(ReportsApi);
  return (filter: MonthlyReportFilter | EmployeesReportFilter) => {
    busy.set(true);
    api
      .exportExcel(kind, filter)
      .pipe(finalize(() => busy.set(false)))
      .subscribe((response) => saveBlobResponse(response, `${kind}.xlsx`));
  };
}
