import { Direction, Directionality } from '@angular/cdk/bidi';
import { DOCUMENT, Injectable, computed, inject, signal } from '@angular/core';
import { DateAdapter } from '@angular/material/core';
import { TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';

export type AppLanguage = 'en' | 'ar';

export interface LanguageOption {
  code: AppLanguage;
  label: string;
  direction: Direction;
  /** BCP 47 locale for Intl formatting; Arabic keeps Latin digits for financial readability. */
  locale: string;
}

const STORAGE_KEY = 'payroll.language';

export const LANGUAGES: readonly LanguageOption[] = [
  { code: 'en', label: 'English', direction: 'ltr', locale: 'en-US' },
  { code: 'ar', label: 'العربية', direction: 'rtl', locale: 'ar-EG-u-nu-latn' },
];

@Injectable({ providedIn: 'root' })
export class LanguageService {
  private readonly translate = inject(TranslateService);
  private readonly document = inject(DOCUMENT);
  private readonly directionality = inject(Directionality);
  private readonly dateAdapter = inject(DateAdapter);

  private readonly language = signal<AppLanguage>(this.detectInitialLanguage());

  readonly languages = LANGUAGES;
  readonly current = this.language.asReadonly();
  readonly option = computed(() => LANGUAGES.find((l) => l.code === this.language())!);
  readonly direction = computed(() => this.option().direction);
  readonly locale = computed(() => this.option().locale);

  /** Loads the initial translations; used by the app initializer so the first paint is translated. */
  init(): Observable<unknown> {
    return this.use(this.language());
  }

  use(code: AppLanguage): Observable<unknown> {
    this.language.set(code);
    localStorage.setItem(STORAGE_KEY, code);
    this.applyToDocument();
    return this.translate.use(code);
  }

  toggle(): void {
    this.use(this.language() === 'en' ? 'ar' : 'en').subscribe();
  }

  private applyToDocument(): void {
    const { code, direction, locale } = this.option();
    const html = this.document.documentElement;
    html.lang = code;
    html.dir = direction;

    // The root Directionality drives overlays (dialogs, menus, snack bars) and the sidenav.
    if (this.directionality.value !== direction) {
      this.directionality.valueSignal.set(direction);
      this.directionality.change.emit(direction);
    }
    this.dateAdapter.setLocale(locale);
  }

  private detectInitialLanguage(): AppLanguage {
    const saved = localStorage.getItem(STORAGE_KEY);
    if (saved === 'en' || saved === 'ar') return saved;
    return navigator.language?.toLowerCase().startsWith('ar') ? 'ar' : 'en';
  }
}
