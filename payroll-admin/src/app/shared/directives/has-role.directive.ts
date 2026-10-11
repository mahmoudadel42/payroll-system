import { Directive, TemplateRef, ViewContainerRef, effect, inject, input } from '@angular/core';
import { Role } from '@core/auth/auth.models';
import { AuthService } from '@core/auth/auth.service';

/** Usage: `*appHasRole="'Admin'"` or `*appHasRole="['Admin', 'HR']"`. UI-only; the API enforces the real check. */
@Directive({ selector: '[appHasRole]' })
export class HasRoleDirective {
  private readonly auth = inject(AuthService);
  private readonly template = inject(TemplateRef<unknown>);
  private readonly container = inject(ViewContainerRef);

  readonly appHasRole = input.required<Role | Role[]>();

  constructor() {
    effect(() => {
      const required = this.appHasRole();
      const allowed = this.auth.hasAnyRole(Array.isArray(required) ? required : [required]);
      this.container.clear();
      if (allowed) this.container.createEmbeddedView(this.template);
    });
  }
}
