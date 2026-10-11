import { computed, inject, linkedSignal, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { PageEvent } from '@angular/material/paginator';
import { Observable } from 'rxjs';
import { LanguageService } from '@core/i18n/language.service';
import { PagedQuery, PagedResult, emptyPage } from '@shared/models/api.models';

export interface PagedResourceOptions<T, Q extends PagedQuery> {
  initialQuery: Q;
  loader: (query: Q) => Observable<PagedResult<T>>;
  /** Re-fetch when the UI language changes because the API localizes some fields (e.g. department names). */
  localized?: boolean;
}

/**
 * Server-side paged list state for table pages. Must be created in an injection context.
 * Keeps showing the previous page while the next one loads to avoid table flicker.
 */
export function pagedResource<T, Q extends PagedQuery>(options: PagedResourceOptions<T, Q>) {
  const language = inject(LanguageService);
  const query = signal<Q>(options.initialQuery);

  const resource = rxResource({
    params: () => {
      if (options.localized) language.current();
      return { ...query() };
    },
    stream: ({ params }) => options.loader(params),
  });

  const data = linkedSignal<PagedResult<T> | undefined, PagedResult<T>>({
    source: () => (resource.hasValue() ? resource.value() : undefined),
    computation: (value, previous) => value ?? previous?.value ?? emptyPage<T>(),
  });

  return {
    query: query.asReadonly(),
    items: computed(() => data().items),
    totalCount: computed(() => data().totalCount),
    pageIndex: computed(() => query().page - 1),
    pageSize: computed(() => query().pageSize),
    isLoading: resource.isLoading,
    hasError: computed(() => resource.status() === 'error'),

    /** Applies filter changes and returns to the first page. */
    filter(changes: Partial<Q>): void {
      query.update((current) => ({ ...current, ...changes, page: 1 }));
    },

    changePage(event: PageEvent): void {
      query.update((current) => ({ ...current, page: event.pageIndex + 1, pageSize: event.pageSize }));
    },

    reload(): void {
      resource.reload();
    },
  };
}

export type PagedResource<T, Q extends PagedQuery> = ReturnType<typeof pagedResource<T, Q>>;
