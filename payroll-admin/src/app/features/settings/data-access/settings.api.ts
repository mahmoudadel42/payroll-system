import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '@core/http/api.config';
import { silentErrors } from '@core/http/http-context';
import {
  AbsenceRule,
  GradeSalary,
  SaveAbsenceRuleRequest,
  SaveServiceYearsRuleRequest,
  ServiceYearsRule,
} from './settings.models';

@Injectable({ providedIn: 'root' })
export class SettingsApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(API_BASE_URL);

  gradeSalaries(): Observable<GradeSalary[]> {
    return this.http.get<GradeSalary[]>(`${this.baseUrl}/gradesalaries`);
  }

  /** The API has separate create/update endpoints; unconfigured grades must be created first. */
  saveGradeSalary(salary: GradeSalary, baseSalary: number): Observable<unknown> {
    const body = { grade: salary.grade, baseSalary };
    return salary.baseSalary === null
      ? this.http.post<number>(`${this.baseUrl}/gradesalaries`, body, { context: silentErrors() })
      : this.http.put<void>(`${this.baseUrl}/gradesalaries/${salary.grade}`, body, { context: silentErrors() });
  }

  serviceYearsRules(): Observable<ServiceYearsRule[]> {
    return this.http.get<ServiceYearsRule[]>(`${this.baseUrl}/serviceyearsrules`);
  }

  createServiceYearsRule(request: SaveServiceYearsRuleRequest): Observable<number> {
    return this.http.post<number>(`${this.baseUrl}/serviceyearsrules`, request, { context: silentErrors() });
  }

  updateServiceYearsRule(id: number, request: SaveServiceYearsRuleRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/serviceyearsrules/${id}`, request, { context: silentErrors() });
  }

  deleteServiceYearsRule(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/serviceyearsrules/${id}`);
  }

  absenceRules(): Observable<AbsenceRule[]> {
    return this.http.get<AbsenceRule[]>(`${this.baseUrl}/absencerules`);
  }

  createAbsenceRule(request: SaveAbsenceRuleRequest): Observable<number> {
    return this.http.post<number>(`${this.baseUrl}/absencerules`, request, { context: silentErrors() });
  }

  updateAbsenceRule(id: number, request: SaveAbsenceRuleRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/absencerules/${id}`, request, { context: silentErrors() });
  }

  deleteAbsenceRule(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/absencerules/${id}`);
  }
}
