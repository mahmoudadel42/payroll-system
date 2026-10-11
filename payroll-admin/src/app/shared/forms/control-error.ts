import { Component, computed, inject, input } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { AbstractControl, ValidationErrors } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { startWith, switchMap } from 'rxjs';

/** Renders the first validation error of a control. Place inside `<mat-error>`. */
@Component({
  selector: 'app-control-error',
  template: `{{ message() }}`,
})
export class ControlError {
  private readonly translate = inject(TranslateService);

  readonly control = input.required<AbstractControl>();

  private readonly controlEvent = toSignal(
    toObservable(this.control).pipe(switchMap((control) => control.events.pipe(startWith(null)))),
  );

  protected readonly message = computed(() => {
    this.controlEvent();
    this.translate.currentLang();
    const errors = this.control().errors;
    return errors ? this.describe(errors) : '';
  });

  private describe(errors: ValidationErrors): string {
    const [key, value] = Object.entries(errors)[0];
    switch (key) {
      case 'server':
        return value as string;
      case 'min':
        return this.translate.instant('validation.min', { min: value.min });
      case 'max':
        return this.translate.instant('validation.max', { max: value.max });
      case 'minlength':
        return this.translate.instant('validation.minlength', { length: value.requiredLength });
      case 'maxlength':
        return this.translate.instant('validation.maxlength', { length: value.requiredLength });
      case 'matDatepickerParse':
        return this.translate.instant('validation.date');
      case 'matDatepickerMax':
        return this.translate.instant('validation.dateMax');
      default:
        return this.translate.instant(`validation.${key}`);
    }
  }
}
