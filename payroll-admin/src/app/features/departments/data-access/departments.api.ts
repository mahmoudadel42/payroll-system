import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '@core/http/api.config';
import { silentErrors } from '@core/http/http-context';
import { PagedQuery, PagedResult } from '@shared/models/api.models';
import { toHttpParams } from '@shared/utils/http-params';
import { Department, SaveDepartmentRequest } from './department.models';

@Injectable({ providedIn: 'root' })
export class DepartmentsApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${inject(API_BASE_URL)}/departments`;

  list(query: PagedQuery): Observable<PagedResult<Department>> {
    return this.http.get<PagedResult<Department>>(this.baseUrl, { params: toHttpParams(query) });
  }

  create(request: SaveDepartmentRequest): Observable<number> {
    return this.http.post<number>(this.baseUrl, request, { context: silentErrors() });
  }

  update(id: number, request: SaveDepartmentRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request, { context: silentErrors() });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
