import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '@core/http/api.config';
import { silentErrors } from '@core/http/http-context';
import { PagedResult } from '@shared/models/api.models';
import { toHttpParams } from '@shared/utils/http-params';
import {
  GeneratePayrollRunRequest,
  PayrollRun,
  PayrollRunDetails,
  PayrollRunsQuery,
  Payslip,
} from './payroll-run.models';

@Injectable({ providedIn: 'root' })
export class PayrollRunsApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${inject(API_BASE_URL)}/payrollruns`;

  list(query: PayrollRunsQuery): Observable<PagedResult<PayrollRun>> {
    return this.http.get<PagedResult<PayrollRun>>(this.baseUrl, { params: toHttpParams(query) });
  }

  get(id: number): Observable<PayrollRunDetails> {
    return this.http.get<PayrollRunDetails>(`${this.baseUrl}/${id}`);
  }

  getPayslip(id: number): Observable<Payslip> {
    return this.http.get<Payslip>(`${this.baseUrl}/payslips/${id}`);
  }

  generate(request: GeneratePayrollRunRequest): Observable<number> {
    return this.http.post<number>(this.baseUrl, request, { context: silentErrors() });
  }

  recalculate(id: number): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/recalculate`, null);
  }

  approve(id: number): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/approve`, null);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
