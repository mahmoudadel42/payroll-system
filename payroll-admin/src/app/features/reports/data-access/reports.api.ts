import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '@core/http/api.config';
import { toHttpParams } from '@shared/utils/http-params';
import {
  AttendanceReport,
  EmployeesReport,
  EmployeesReportFilter,
  IncentivesDeductionsReport,
  MonthlyReportFilter,
  ReportKind,
  SalariesReport,
} from './report.models';

@Injectable({ providedIn: 'root' })
export class ReportsApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${inject(API_BASE_URL)}/reports`;

  salaries(filter: MonthlyReportFilter): Observable<SalariesReport> {
    return this.http.get<SalariesReport>(`${this.baseUrl}/salaries`, { params: toHttpParams(filter) });
  }

  incentivesDeductions(filter: MonthlyReportFilter): Observable<IncentivesDeductionsReport> {
    return this.http.get<IncentivesDeductionsReport>(`${this.baseUrl}/incentives-deductions`, {
      params: toHttpParams(filter),
    });
  }

  attendance(filter: MonthlyReportFilter): Observable<AttendanceReport> {
    return this.http.get<AttendanceReport>(`${this.baseUrl}/attendance`, { params: toHttpParams(filter) });
  }

  employees(filter: EmployeesReportFilter): Observable<EmployeesReport> {
    return this.http.get<EmployeesReport>(`${this.baseUrl}/employees`, { params: toHttpParams(filter) });
  }

  /** Same endpoints with `format=xlsx`; the API returns the workbook as a file download. */
  exportExcel(kind: ReportKind, filter: MonthlyReportFilter | EmployeesReportFilter): Observable<HttpResponse<Blob>> {
    return this.http.get(`${this.baseUrl}/${kind}`, {
      params: toHttpParams({ ...filter, format: 'xlsx' }),
      responseType: 'blob',
      observe: 'response',
    });
  }
}
