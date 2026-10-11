import { HttpClient } from '@angular/common/http';
import { Injectable, Signal, computed, inject } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '@core/http/api.config';
import { LanguageService } from '@core/i18n/language.service';
import { Lookup } from '@shared/models/api.models';

@Injectable({ providedIn: 'root' })
export class LookupsApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${inject(API_BASE_URL)}/lookups`;

  /** Names are localized by the API according to the active language. */
  departments(): Observable<Lookup[]> {
    return this.http.get<Lookup[]>(`${this.baseUrl}/departments`);
  }
}

/** Department options that refresh when the language changes. Call in an injection context. */
export function injectDepartmentOptions(): Signal<Lookup[]> {
  const api = inject(LookupsApi);
  const language = inject(LanguageService);
  const resource = rxResource({
    params: () => language.current(),
    stream: () => api.departments(),
  });
  return computed(() => (resource.hasValue() ? resource.value() : []));
}
