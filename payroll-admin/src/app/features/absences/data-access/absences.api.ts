import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '@core/http/api.config';
import { silentErrors } from '@core/http/http-context';
import { PagedResult } from '@shared/models/api.models';
import { toHttpParams } from '@shared/utils/http-params';
import { Absence, AbsencesQuery, CreateAbsencesRequest, UpdateAbsenceRequest } from './absence.models';

@Injectable({ providedIn: 'root' })
export class AbsencesApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${inject(API_BASE_URL)}/absences`;

  list(query: AbsencesQuery): Observable<PagedResult<Absence>> {
    return this.http.get<PagedResult<Absence>>(this.baseUrl, { params: toHttpParams(query) });
  }

  /** Records several days at once; returns how many were created (already-recorded dates are skipped). */
  createMany(request: CreateAbsencesRequest): Observable<number> {
    return this.http.post<number>(`${this.baseUrl}/bulk`, request, { context: silentErrors() });
  }

  update(id: number, request: UpdateAbsenceRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request, { context: silentErrors() });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
