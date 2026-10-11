import { Component, computed, inject, input } from '@angular/core';
import { rxResource, takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { ErrorStateMatcher } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TranslatePipe } from '@ngx-translate/core';
import { debounceTime, distinctUntilChanged, map, of } from 'rxjs';
import { ControlError } from '@shared/forms/control-error';
import { Lookup } from '@shared/models/api.models';
import { EmployeesApi } from '../../data-access/employees.api';

/**
 * Searchable employee selector bound to a `FormControl<Lookup | null>`.
 * Typing clears the selection until an option is picked.
 */
@Component({
  selector: 'app-employee-picker',
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatAutocompleteModule, MatIconModule, TranslatePipe, ControlError],
  template: `
    <mat-form-field class="picker">
      <mat-label>{{ label() | translate }}</mat-label>
      <input
        matInput
        [formControl]="text"
        [matAutocomplete]="auto"
        [errorStateMatcher]="errorMatcher"
        (blur)="control().markAsTouched()"
      />
      <mat-icon matPrefix>person_search</mat-icon>
      <mat-autocomplete #auto [displayWith]="displayName" (optionSelected)="select($event)">
        @for (employee of options(); track employee.id) {
          <mat-option [value]="employee">
            {{ employee.name }}
          </mat-option>
        }
      </mat-autocomplete>
      <mat-error><app-control-error [control]="control()" /></mat-error>
    </mat-form-field>
  `,
  styles: `
    .picker {
      width: 100%;
    }
  `,
})
export class EmployeePicker {
  private readonly api = inject(EmployeesApi);

  readonly control = input.required<FormControl<Lookup | null>>();
  readonly label = input('absences.employee');

  protected readonly text = new FormControl<string | Lookup>('', { nonNullable: true });

  protected readonly errorMatcher: ErrorStateMatcher = {
    isErrorState: () => this.control().invalid && this.control().touched,
  };

  private readonly search = toSignal(
    this.text.valueChanges.pipe(
      map((value) => (typeof value === 'string' ? value.trim() : null)),
      debounceTime(250),
      distinctUntilChanged(),
    ),
    { initialValue: '' },
  );

  private readonly results = rxResource({
    params: () => this.search(),
    stream: ({ params }) =>
      params === null
        ? of([])
        : this.api
            .list({ page: 1, pageSize: 20, search: params })
            .pipe(map((page) => page.items.map((e): Lookup => ({ id: e.id, name: e.fullName })))),
  });

  protected readonly options = computed(() => (this.results.hasValue() ? this.results.value() : []));

  constructor() {
    this.text.valueChanges.pipe(takeUntilDestroyed()).subscribe((value) => {
      if (typeof value === 'string' && this.control().value) this.control().setValue(null);
    });
  }

  protected readonly displayName = (value: Lookup | string | null): string =>
    typeof value === 'string' ? value : (value?.name ?? '');

  protected select(event: MatAutocompleteSelectedEvent): void {
    this.control().setValue(event.option.value as Lookup);
    this.control().markAsTouched();
  }
}
