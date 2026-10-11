import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '@core/http/api.config';
import { silentErrors } from '@core/http/http-context';
import { PagedResult } from '@shared/models/api.models';
import { toHttpParams } from '@shared/utils/http-params';
import { Employee, EmployeeDetails, EmployeesQuery, SaveEmployeeRequest } from './employee.models';

@Injectable({ providedIn: 'root' })
export class EmployeesApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${inject(API_BASE_URL)}/employees`;

  list(query: EmployeesQuery): Observable<PagedResult<Employee>> {
    return this.http.get<PagedResult<Employee>>(this.baseUrl, { params: toHttpParams(query) });
  }

  get(id: number): Observable<EmployeeDetails> {
    return this.http.get<EmployeeDetails>(`${this.baseUrl}/${id}`);
  }

  create(request: SaveEmployeeRequest): Observable<number> {
    return this.http.post<number>(this.baseUrl, request, { context: silentErrors() });
  }

  update(id: number, request: SaveEmployeeRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request, { context: silentErrors() });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
