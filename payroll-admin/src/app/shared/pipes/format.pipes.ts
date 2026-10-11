import { Pipe, PipeTransform, inject } from '@angular/core';
import { LanguageService } from '@core/i18n/language.service';
import { fromIsoDate } from '@shared/utils/date';

// Pipes are impure so output follows runtime language switches; Intl formatters are cached per locale.
const cache = new Map<string, Intl.NumberFormat | Intl.DateTimeFormat>();

function formatter<T extends Intl.NumberFormat | Intl.DateTimeFormat>(key: string, create: () => T): T {
  if (!cache.has(key)) cache.set(key, create());
  return cache.get(key) as T;
}

@Pipe({ name: 'money', pure: false })
export class MoneyPipe implements PipeTransform {
  private readonly language = inject(LanguageService);

  transform(value: number | null | undefined): string {
    if (value === null || value === undefined) return '—';
    const locale = this.language.locale();
    return formatter(`money|${locale}`, () =>
      new Intl.NumberFormat(locale, { minimumFractionDigits: 2, maximumFractionDigits: 2 }),
    ).format(value);
  }
}

/** Formats a percentage stored as 0–100 (the API convention). */
@Pipe({ name: 'percentValue', pure: false })
export class PercentValuePipe implements PipeTransform {
  private readonly language = inject(LanguageService);

  transform(value: number | null | undefined): string {
    if (value === null || value === undefined) return '—';
    const locale = this.language.locale();
    return formatter(`percent|${locale}`, () =>
      new Intl.NumberFormat(locale, { style: 'percent', maximumFractionDigits: 2 }),
    ).format(value / 100);
  }
}

@Pipe({ name: 'localDate', pure: false })
export class LocalDatePipe implements PipeTransform {
  private readonly language = inject(LanguageService);

  transform(value: string | null | undefined, style: 'date' | 'dateTime' = 'date'): string {
    if (!value) return '—';
    const date = style === 'date' ? fromIsoDate(value) : new Date(value);
    if (!date || isNaN(date.getTime())) return '—';
    const locale = this.language.locale();
    return formatter(`date|${style}|${locale}`, () =>
      new Intl.DateTimeFormat(locale, {
        dateStyle: 'medium',
        ...(style === 'dateTime' ? { timeStyle: 'short' } : {}),
      }),
    ).format(date);
  }
}

/** Renders a payroll period such as "October 2026" / "أكتوبر 2026". */
@Pipe({ name: 'period', pure: false })
export class PeriodPipe implements PipeTransform {
  private readonly language = inject(LanguageService);

  transform(year: number | null | undefined, month: number | null | undefined): string {
    if (!year || !month) return '—';
    const locale = this.language.locale();
    return formatter(`period|${locale}`, () =>
      new Intl.DateTimeFormat(locale, { month: 'long', year: 'numeric' }),
    ).format(new Date(year, month - 1, 1));
  }
}
